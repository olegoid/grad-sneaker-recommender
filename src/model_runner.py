import sys
import json
import numpy as np
import faiss
from PIL import Image
from torchvision import transforms
from efficientnet_pytorch import EfficientNet

# Load EfficientNet-B0
model = EfficientNet.from_pretrained('efficientnet-b0')
model.eval()

# Preprocessing
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
        features = model.extract_features(input_tensor).squeeze().numpy()
    return features / np.linalg.norm(features)

def query_faiss(image_embedding, top_k=5):
    D, I = faiss_index.search(np.expand_dims(image_embedding, axis=0), top_k)
    return [metadata[str(idx)] for idx in I[0]]

if __name__ == "__main__":
    image_path = sys.argv[1]
    text = sys.argv[2] if len(sys.argv) > 2 else None  # Unused here, but for multimodal

    emb = get_embedding(image_path)
    results = query_faiss(emb)

    response = {
        "recommendation": f"Top {len(results)} similar sneakers found.",
        "image_url": results[0]["image_url"]  # assume metadata contains image_url
    }
    print(json.dumps(response))
