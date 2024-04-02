### Project Overview

This project aims to develop an advanced system for the comparative analysis and identification of similar sneaker designs. Utilizing cutting-edge technologies such as LLM-s and graph DB-s, the platform will automate the process of collecting, analyzing, and comparing sneaker designs from leading manufacturers.

### Task Statement
The primary objective is to create a system capable of scraping sneaker images from the web, analyzing these images to generate detailed descriptions, and storing this information in a graph database for comparative analysis. At first the project will heavily rely on OpenAI-s public API for description generation. However, the primary research objective of this work is to develop a model equivalent to ChatGPT capable of description generation.

- Author: Oleg Demchenko
- Scientific Advisor: Georgy Panchuk

### Execution Plan

##### Web Scraper Development

Develop a scraper to collect sneaker images from specified websites.

##### Database Design and Implementation (Neo4j)

Design the Neo4j graph database schema. Implement the database to store sneaker descriptions. At the time of writing I have the following schema in mind:

```
{
  "sneaker": {
    "sneakerId": "unique_sneaker_id",
    "name": "Sneaker Model Name",
    "brand": "Sneaker Brand",
    "components": [
      {
        "nodeType": "OverallShape",
        "description": "Shape description"
      },
      {
        "nodeType": "Upper",
        "materials": ["Material1", "Material2"],
        "colorway": ["Color1", "Color2"],
        "pattern": "Pattern description"
      },
      {
        "nodeType": "Midsole",
        "material": "Midsole material",
        "color": "Midsole color",
        "technology": "Cushioning tech"
      },
      {
        "nodeType": "Outsole",
        "material": "Outsole material",
        "color": "Outsole color",
        "pattern": "Tread pattern"
      },
      {
        "nodeType": "Laces",
        "color": "Lace color",
        "material": "Lace material",
        "pattern": "Lace pattern"
      }
    ],
    "images": [
      "http://example.com/image1.jpg",
      "http://example.com/image2.jpg"
    ],
    "tags": ["Retro", "Limited Edition", "Performance"]
  }
}
```

Example description:



```
{
  "sneaker": {
    "sneakerId": "NIKE-AIR-MAX-270-001",
    "name": "Nike Air Max 270 Women's Shoes",
    "brand": "Nike",
    "components": [
      {
        "nodeType": "OverallShape",
        "description": "Low-top silhouette with an asymmetrical lacing system and a bootie-like construction for a snug fit."
      },
      {![air-max-270-womens-shoes-Pgb94t](https://github.com/olegoid/grad-sneaker-recommender/assets/1524073/32a5a7fa-b7e9-4aeb-9d4a-79e312aa52b1)

        "nodeType": "Upper",
        "materials": [
          "Engineered mesh for breathability",
          "Neoprene for stretch and comfort",
          "Synthetic overlays for structure"
        ],
        "colorway": ["White", "Pink Foam", "Yellow Strike", "Black"],
        "pattern": "The upper has a white base with a subtle diamond pattern, accented with black Nike Swoosh, Pink Foam heel tab, and Yellow Strike lace loops."
      },
      {
        "nodeType": "Midsole",
        "material": "Dual-density foam",
        "color": "White with a clear Air Max unit exposing a vibrant Hot Punch color",
        "technology": "Max Air 270 unit for superior heel cushioning and a smooth transition to the forefoot."
      },
      {
        "nodeType": "Outsole",
        "material": "Rubber",
        "color": "Hot Punch to Black gradient with strategically placed traction pads",
        "pattern": "Dual-tone outsole with waffle-inspired pattern for durability and grip."
      },
      {
        "nodeType": "Laces",
        "color": "Yellow Strike",
        "material": "Woven fibers",
        "pattern": "Flat laces designed to stay tied during active use."
      },
      {
        "nodeType": "Heel Counter",
        "material": "Thermoplastic polyurethane (TPU)",
        "color": "Translucent Pink Foam",
        "feature": "Sturdy external heel counter provides support and locks the heel in place."
      },
      {
        "nodeType": "Sockliner",
        "material": "EVA foam",
        "feature": "Cushioned sockliner conforms to the foot for a custom fit."
      },
      {
        "nodeType": "Tongue",
        "material": ["Neoprene", "Mesh"],
        "color": "White with a hint of Yellow Strike",
        "feature": "Padded tongue with 'Air 270' branding offers cushioning over the foot arch."
      }
    ],
    "images": [
      "http://example.com/air-max-270-womens-shoes-Pgb94t.png"
    ],
    "tags": ["Sporty", "Vibrant", "Air Cushioning", "Iconic Design"]
  }
}
```


##### Integration with ChatGPT API

Use the ChatGPT API to generate textual descriptions of the sneakers.

##### Development of Custom LLM (MiniGPT4 + fine tuning)

- [ ] Step 1: Preparing the Dataset
Data Collection: Deploy a web scraper to gather a large dataset of sneaker images from various manufacturer websites such as Nike, Adidas, Puma, etc. Ensure diversity in the dataset to cover a wide range of sneaker designs.
Generating Descriptions with ChatGPT: For each collected image, use the ChatGPT API to generate detailed textual descriptions. These descriptions should focus on identifying distinctive sneaker features, such as color, pattern, material, and unique design elements.
Data Cleaning and Structuring: Process the image-description pairs to remove duplicates and ensure the quality of the data. Store the pairs in a structured format, adhering to a predefined JSON schema for consistency.

- [ ] Step 2: Initial Training Phase
Model Architecture: Integrate a pretrained vision encoder (e.g., ViT-G/14 from EVA-CLIP) with a frozen instance of an advanced LLM (e.g., Vicuna), using a single linear projection layer. This configuration is designed to align the visual features extracted from the sneaker images with the textual understanding of the Vicuna model.
Initial Model Training: Train your model on the prepared dataset. The objective at this stage is for the model to learn the correspondence between the visual features of sneakers and their textual descriptions generated by ChatGPT.

- [ ] Step 3: Curating a High-Quality Dataset
Enhancing Data Quality: Post initial training, identify areas where the model’s descriptions could be improved. Generate additional descriptions for images where the initial text was found lacking, using either refined prompts with ChatGPT or manual curation.
Dataset Refinement: Create a high-quality dataset of image-description pairs by selecting the best examples from the generated descriptions. This dataset should focus on detailed, accurate, and diverse descriptions that can challenge and thus improve the model.

- [ ] Step 4: Fine-tuning with Advanced Dataset
Fine-tuning Process: Utilize the curated high-quality dataset to fine-tune your model. This stage aims to enhance the model's ability to generate precise and natural descriptions by learning from the most accurate and detailed examples.
Adjusting Training Parameters: Based on initial fine-tuning results, adjust the model's training parameters, such as learning rate, batch size, and the number of epochs, to optimize performance.

- [ ] Step 5: Evaluation and Iteration
Performance Evaluation: Evaluate the fine-tuned model’s performance using a separate test set not seen by the model during training. Employ both qualitative and quantitative assessments to measure the accuracy and naturalness of the generated descriptions.
Iterative Refinement: Based on performance feedback, iterate over the fine-tuning process, making necessary adjustments to the model and possibly enriching the high-quality dataset with more examples.

##### Comparative Analysis Tool Development

Develop the final product for comparing sneaker designs.
