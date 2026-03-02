# Modular Minimal API .NET 10 — Companion Labs

Welcome to the official repository for the labs and source code of the book:
**"Modular Minimal API .NET 10: A Step-by-Step Guide to Building Professional, Scalable, and Production-Ready Backend Systems"** by Afandy Lamusu.

This repository is designed to be your practical companion as you progress through the book. Each folder in the `labs/` directory corresponds to a specific chapter, allowing you to see the "final state" of the project at the end of that chapter.

## 🚀 Tech Stack

- **Framework:** .NET 10 (Minimal APIs)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Architecture:** Modular Monolith (Clean Architecture principles)
- **Tooling:** Docker (for PostgreSQL), FluentValidation, Serilog

## 🛠 Prerequisites

Before starting, ensure you have the following installed on your machine:

1.  **.NET 10 SDK:** [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
2.  **Docker Desktop:** Used for running the PostgreSQL database.
3.  **IDE:** Visual Studio 2022 (v17.12+), VS Code, or JetBrains Rider.
4.  **HTTP Client:** Postman, Insomnia, or the built-in REST Client in your IDE.

## 📖 How to Use the Labs

The labs are structured to follow the book's narrative. We recommend following these steps:

1.  **Clone the Repository:**
    ```bash
    git clone https://github.com/afandy-lamusu/labs-modular-minimal-api-net10.git
    cd labs-modular-minimal-api-net10
    ```

2.  **Navigate to a Chapter:**
    Each chapter folder is self-contained. For example, to start with Chapter 4 (Project Setup):
    ```bash
    cd labs/04-Setup
    ```

3.  **Spin up the Infrastructure:**
    Most chapters include a `docker-compose.yml` file to start the PostgreSQL database:
    ```bash
    docker-compose up -d
    ```

4.  **Run the Project:**
    Navigate to the API project folder and run it:
    ```bash
    cd ModularStore.Api
    dotnet run
    ```

5.  **Compare and Learn:**
    Use these labs to verify your work if you get stuck while following the book's instructions. Each lab represents a milestone in the development of the **ModularStore** application.

## 📂 Repository Structure

- `labs/04-Setup/`: Initial project structure and database configuration.
- `labs/05-Product-Module/`: Implementation of the first module (Products).
- `labs/06-Orders-Module/`: Adding the second module and internal communication.
- `labs/10-Final/`: The complete Modular Monolith with Auth, Logging, and Production settings.
- `labs/11-Microservices/`: The result of extracting a module into a standalone service.
- `labs/12-Automated-Testing/`: Unit and Architecture tests for the solution.

## 📜 License

**Copyright © 2026 Afandy Lamusu / Afandy.Lamusu Tech Publishing. All rights reserved.**

The code in this repository is provided for educational purposes as a companion to the book "Modular Minimal API .NET 10". 

- **Book Content:** No part of the book or its textual instructions may be reproduced or transmitted without prior written permission from the publisher.
- **Source Code:** You are free to use, modify, and distribute the code examples for your own learning and personal projects. For commercial use or redistribution as part of a training program, please contact the author.

---
*Happy Coding!*
**Afandy Lamusu**
