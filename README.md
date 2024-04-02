### Project Overview

This project aims to develop an advanced system for the comparative analysis and identification of similar sneaker designs. Utilizing cutting-edge technologies such as LLM-s and graph DB-s, the platform will automate the process of collecting, analyzing, and comparing sneaker designs from leading manufacturers.

### Task Statement
The primary objective is to create a system capable of scraping sneaker images from the web, analyzing these images to generate detailed descriptions, and storing this information in a graph database for comparative analysis. At first the project will heavily rely on OpenAI-s public API for description generation. However, the primary research objective of this work is to develop a model equivalent to ChatGPT capable of description generation.

- Author: Oleg Demchenko
- Scientific Advisor: Georgy Panchuk


### Execution Plan

##### Web Scraper Development

Develop a scraper to collect sneaker images from specified websites.

##### Integration with ChatGPT API

Use the ChatGPT API to generate textual descriptions of the sneakers.

##### Database Design and Implementation (Neo4j)

Design the Neo4j graph database schema. Implement the database to store sneaker descriptions.

##### Development of Custom LLM (MiniGPT4 + fine tuning)

Train a custom language model for generating sneaker descriptions to replace ChatGPT.

##### Comparative Analysis Tool Development

Develop the final product for comparing sneaker designs.
