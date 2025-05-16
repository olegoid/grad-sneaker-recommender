# Sneaker Recommendation Platform

This project is a prototype of a recommendation platform that enables users to find visually similar sneakers based on an uploaded photo, optionally enriched by a textual description (e.g., color, style, or brand). Example of how the recommendation system works with a Telegram Bot as a client:

![unnamed (1)](https://github.com/user-attachments/assets/1c34866d-9d15-4f4d-8b4b-ed11fa998e96)

## Overview

The system uses a combination of deep learning models — specifically EfficientNet-B0 and DistilBERT — to extract image and text embeddings. It supports image-based search, text-conditioned ranking, and hybrid queries (image + text). A Telegram bot serves as the primary user interface for ease of access and deployment.

## Features

- 🔍 Visual search: Upload a photo of sneakers and get similar-looking results.
- 📝 Text-conditioned recommendations: Add style/brand/color preferences as a query.
- 🧠 Multimodal embedding: Combine image and text into a unified recommendation space.

## Technologies

- **Computer Vision**: EfficientNet-B0 with and without triplet loss (trained and evaluated in Python notebooks)
- **NLP**: DistilBERT for text embedding and hybrid search (evaluated in Python notebooks)
- **Infrastructure**: Azure ML for model hosting, Azure Functions and Message Queue for task orchestration, Azure Blob Storage for image data, PostgreSQL for metadata storage
- **Backend Services**: .NET (C#) microservices for API handling, inference orchestration, and integration with Azure components
- **Interface**: Telegram bot integrated with .NET backend for user interaction
- **Recommendation Engine**: Embedding-based nearest neighbor search using FAISS, evaluated and exported via Python

## Project Structure

- `model/` – Training scripts and model definitions for EfficientNet-B0 and DistilBERT
- `api/` – FastAPI-based microservices for preprocessing and orchestrating inference
- `ml_service/` – Azure ML deployment specs for inference containers
- `telegram_bot/` – Telegram integration for user interaction
- `infrastructure/` – Terraform/ARM templates for cloud resource provisioning

## How It Works



![unnamed](https://github.com/user-attachments/assets/6fdea66f-3db6-4503-a01a-bbfc42d6efe3)

1. User uploads a sneaker image via the Telegram bot, optionally with a textual query.
2. The request is processed and sent to a message queue.
3. ML service consumes the queue, runs inference to generate image and text embeddings.
4. Embeddings are searched against a product database to find visually and semantically similar sneakers.
5. The top matches are returned and displayed to the user.

## Results

The system achieved high-quality recommendations on test datasets, with notable improvements when combining image and text modalities. Using triplet loss improved embedding quality, while the multimodal EfficientNet-B0 + DistilBERT pipeline enabled expressive and flexible search scenarios.

## Future Work

- Incorporate user behavior data for personalized ranking
- Expand product categories beyond sneakers
- Refine multilingual and domain-specific language support
- Optimize embedding indexing and update pipelines

---

> Created by Oleg Demchenko as part of a master's thesis at HSE University, Faculty of Computer Science.
