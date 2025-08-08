# OnlineRetailer.Staff

A frontend service for staff management within the OnlineRetailer system, developed as part of a university Cloud Computing DevOps module.

---

## Overview

This repository provides a staff management interface for the OnlineRetailer platform. It enables staff users to view, create, edit, and delete products via a secure web interface, with all operations routed through a resilient API client. The application demonstrates DevOps best practices and modern ASP.NET Core design.

---


<img width="2791" height="1960" alt="466094100-f0320752-d0df-4d00-8815-c88bce37b46e" src="https://github.com/user-attachments/assets/53a3dc3c-9d8d-4f25-b29d-4352ea55af6c" />




## Project Structure

- **ThAmCo.Staff/**
  - `Controllers/` – Handles incoming requests and routes for product management (MVC)
  - `Models/` – Domain models for products and data transfer
  - `Services/` – Business logic and API client for backend communication
  - `Views/` – User interface views and templates (Razor)
  - `wwwroot/` – Static files (CSS, JavaScript, images)
  - `Program.cs` – Application start-up and configuration
  - `appsettings.json` – Application configuration

- **Thamco.StaffTests/**  
  Unit and integration tests for the frontend logic

- **.github/**  
  GitHub Actions workflows for CI/CD

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download)
- (Optional) Docker for containerised development

### Setup

1. Clone the repository:
    ```bash
    git clone https://github.com/Meowmixforme/OnlineRetailer.Staff.git
    cd OnlineRetailer.Staff
    ```
2. Navigate to the staff project:
    ```bash
    cd ThAmCo.Staff
    ```
3. Restore dependencies and run the service:
    ```bash
    dotnet restore
    dotnet run
    ```
4. The frontend should now be running on `http://localhost:5000` (default port, configurable in `appsettings.json`).

### Running Tests

Navigate to the test project and run:

```bash
dotnet test Thamco.StaffTests
```

---

## Features

- Secure staff login using Auth0 authentication
- Product listing and detail view for staff
- Create, edit, and delete products through a dedicated interface
- Resilient backend communication with retry and circuit breaker policies (Polly)
- Automated unit and integration tests for the API client
- GitHub Actions workflows for automated build and test
- Modern ASP.NET Core MVC architecture

---

## DevOps & CI/CD

- **GitHub Actions** under `.github/` for automated build and test on every push and pull request
- **Configuration management** using `appsettings.json` and environment variables for development and deployment settings

---

## System Architecture & Technology Choices

- **Tech Stack:** ASP.NET Core 6, Auth0 (OIDC), Polly, Razor Views
- **Architecture:** MVC pattern with clear separation of controllers, services, and views
- **Resilience:** API calls to backend services use retry and circuit breaker policies for reliability

---

## Licence

This project is licensed under the terms of the MIT Licence.
