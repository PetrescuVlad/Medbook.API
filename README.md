# MedBook API

MedBook is a RESTful backend application built with ASP.NET Core that manages doctors, patients, and medical appointments.

The API provides full CRUD operations and includes automatic API documentation through Swagger. The application uses SQLite for data persistence and can be easily deployed using Docker and Docker Compose.

---

# Main Features

- Doctor management (create, list, view details)
- Patient management with each patient associated with a doctor
- Medical appointment management:
  - create
  - update
  - cancel
  - prevention of scheduling conflicts
- Business validations and proper HTTP status codes (e.g. **409 Conflict**)
- Automatic API documentation using Swagger
- Data persistence using SQLite
- Easy deployment using Docker and Docker Compose

---

# Technologies Used

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQLite
- Swagger
- Docker & Docker Compose
- Git & GitHub

---

# Running the Project

You can run the application using Docker.

1. Build the Docker containers

```bash
docker-compose up --build
