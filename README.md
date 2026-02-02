MedBook API

MedBook este o aplicație backend REST dezvoltată în **ASP.NET Core**, care gestionează doctori, pacienți și programări medicale. Aplicația oferă operații CRUD complete prin intermediul unei interfețe Swagger și este containerizată folosind Docker.

Funcționalități principale

- Gestionarea doctorilor (creare, listare, detalii)
- Gestionarea pacienților, fiecare pacient fiind asociat unui doctor
- Gestionarea programărilor medicale:
  - creare
  - modificare
  - anulare
  - prevenirea suprapunerilor
- Validări de business și coduri HTTP corecte (ex: `409 Conflict`)
- Documentație automată prin Swagger
- Persistența datelor folosind SQLite
- Rulare facilă folosind Docker & Docker Compose


Tehnologii utilizate

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQLite
- Swagger 
- Docker & Docker Compose
- Git & GitHub
