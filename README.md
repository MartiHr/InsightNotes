# InsightNotes

InsightNotes is a smart note-taking application that leverages vector search to provide powerful semantic search capabilities. It combines Qdrant for vector storage and search, GraphQL for flexible API queries and mutations, and Hugging Face for generating embeddings from note content.

Features
Add, update, delete, and fetch notes using a clean GraphQL API.

Semantic search powered by vector embeddings — find notes based on meaning, not just keywords.

Scalable vector storage and similarity search using Qdrant.

## 📄 API Overview (GraphQL)

### ➕ Add a Note

```graphql
mutation {
  addNoteAsync(title: "GraphQL Rocks", content: "It’s a powerful query language") {
    id
    title
  }
}
```
### 📋 Get All Notes

```graphql
query {
  getNotesAsync {
    id
    title
    content
    createdAt
  }
}
```

### 🔎 Semantic Search
```graphql
query {
  searchNotesAsync(query: "query languages for APIs") {
    id
    title
    content
    score
  }
}
```

## 🧠 How Semantic Search Works
✉️ You submit a search query like: "efficient note storage".

💡 The query is sent to Hugging Face, which returns a sentence embedding vector.

🧲 That vector is passed to Qdrant, which performs a cosine similarity search over all stored note vectors.

📚 The API returns the most semantically relevant notes based on meaning, not just keywords.


## Technologies Used:

Qdrant — Vector similarity search engine and storage.

GraphQL — API query and mutation language.

HotChocolate – GraphQL server

Hugging Face — Embedding generation via pre-trained NLP models.

.NET (C#) — Backend service implementation.
