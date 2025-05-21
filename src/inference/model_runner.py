import sys
import os
import json
import torch
import numpy as np
import faiss
from PIL import Image
from azure.identity import DefaultAzureCredential
from azure.ai.ml import MLClient
from torchvision import transforms
from efficientnet_pytorch import EfficientNet

# Azure ML model registry settings
AML_SUBSCRIPTION_ID = os.environ.get("AML_SUBSCRIPTION_ID")
AML_RESOURCE_GROUP = os.environ.get("AML_RESOURCE_GROUP")
AML_WORKSPACE_NAME = os.environ.get("AML_WORKSPACE_NAME")
MODEL_NAME = "model"
MODEL_VERSION = "1"

# Authenticate and connect to Azure ML workspace
credential = DefaultAzureCredential()
ml_client = MLClient(
    credential,
    AML_SUBSCRIPTION_ID,
    AML_RESOURCE_GROUP,
    AML_WORKSPACE_NAME,
)

# Download the model from the registry
model = ml_client.models.download(
    name=MODEL_NAME,
    version=MODEL_VERSION,
    download_path="downloaded_model",
)

# Load EfficientNet-B0 weights
model_file = os.path.join("downloaded_model", "efficientnet_b0.pth")
effnet = EfficientNet.from_name("efficientnet-b0")
state_dict = torch.load(model_file, map_location=torch.device("cpu"))
effnet.load_state_dict(state_dict)
effnet.eval()

# Preprocessing pipeline
transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize([0.485, 0.456, 0.406],
                         [0.229, 0.224, 0.225]),
])

# Load FAISS index and metadata
faiss_index = faiss.read_index("faiss_index.index")
with open("index_metadata.json", "r") as f:
    metadata = json.load(f)

def get_embedding(image_path):
    image = Image.open(image_path).convert("RGB")
    input_tensor = transform(image).unsqueeze(0)
    with torch.no_grad():
        features = effnet.extract_features(input_tensor).squeeze().numpy()
    return features / np.linalg.norm(features)

def query_faiss(image_embedding, top_k=5):
    D, I = faiss_index.search(np.expand_dims(image_embedding, axis=0), top_k)
    return [metadata[str(idx)] for idx in I[0]]

if __name__ == "__main__":
    image_path = sys.argv[1]
    text = sys.argv[2] if len(sys.argv) > 2 else None  # Optional

    emb = get_embedding(image_path)
    results = query_faiss(emb)

    response = {
        "recommendation": f"Top {len(results)} similar sneakers found.",
        "image_url": results[0]["image_url"]
    }
    print(json.dumps(response))
