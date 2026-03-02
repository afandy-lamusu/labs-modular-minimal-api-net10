# Modular Minimal API .NET 10 - Sample Projects

This directory contains progressive snapshots of the project as built throughout the book.

## Directory Structure

- **04-Setup**: The starting point. Project creation, PostgreSQL setup with Docker, and initial `DbContext`.
- **05-Product-Module**: Implementation of the first module (Products) including Domain, Application, Infrastructure, and Endpoints.
- **10-Final**: The complete, production-ready application. Includes:
    - Products and Orders modules.
    - Cross-module communication via contracts.
    - Structured logging with Serilog.
    - Global exception handling.
    - snake_case database naming conventions.
    - Dockerfile for production deployment.

## How to Run

1.  Navigate to a sample directory (e.g., `cd 10-Final`).
2.  Start the database: `docker-compose up -d`.
3.  Run the application: `dotnet run --project ModularStore.Api`.
4.  Apply migrations (if `dotnet-ef` is installed): `dotnet ef database update --project ModularStore.Api`.

---
*Author & Architect: **Afandy Lamusu** / Afandy.Lamusu Tech Publishing. Built with love by Afandy & Gemini CLI*
