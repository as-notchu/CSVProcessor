# CSVProcessor API

**Version:** v1  
**Description:** API Uploading and managing movie data

## 📌 Overview

CSVProcessor API allows users to manage movie records by uploading CSV files, viewing, creating, updating, and deleting individual movie entries.

## 🚀 Features

- Upload CSV file to store film data in the database.
- Export all stored films as a CSV file.
- CRUD operations on individual film records via REST endpoints.

## 🛠️ Endpoints

### `/api/Csv`

- `POST`: Upload a CSV file containing film data.
  - **Consumes:** `multipart/form-data`
  - **Returns:** `200 OK`, `400 Bad Request`, `500 Internal Server Error`

- `GET`: Download all film data in CSV format.
  - **Returns:** `200 OK`, `400 Bad Request`, `500 Internal Server Error`

### `/api/Films`

- `GET`: Retrieve all films.
  - **Returns:** List of `FilmData` objects.

- `POST`: Create a new film from a DTO.
  - **Body:** `FilmDTO`
  - **Returns:** `201 Created`, `409 Conflict`, `500 Internal Server Error`

### `/api/Films/{id}`

- `GET`: Get a specific film by its UUID.
  - **Returns:** `200 OK`, `404 Not Found`

- `PUT`: Update film data by UUID.
  - **Body:** `FilmDTO`
  - **Returns:** `201 Created`, `404 Not Found`, `500 Internal Server Error`

- `DELETE`: Delete a film by UUID.
  - **Returns:** `200 OK`, `404 Not Found`

## 🧾 Data Models

### FilmDTO

```json
{
  "title": "string",
  "budget": "string",
  "releaseDate": "string"
}
```

### FilmData

```json
{
  "id": "uuid",
  "title": "string",
  "budget": "string",
  "releaseDate": "string"
}
```

## 📦 Installation & Running

> Find out by youself xD

## 📫 Contributing

1. Fork the repository.
2. Create a new branch.
3. Make your changes.
4. Submit a pull request.



