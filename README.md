# OnlineRetailer.Staff

A frontend service for staff management within the OnlineRetailer system, developed as part of my university DevOps module.

## Overview

This repository provides the staff management frontend for the OnlineRetailer platform. It enables staff users to interact with personnel data, roles, and permissions, offering a user interface for managing staff operations.

## Project Structure

- **ThAmCo.Staff/**  
  Main frontend project for staff management.
  - `Controllers/` – Handles incoming requests and routes (for MVC or API endpoints)
  - `Models/` – Domain models for staff, roles, etc.
  - `Services/` – Business logic and service classes
  - `Views/` – User interface views and templates
  - `Properties/` – Project properties and launch settings
  - `wwwroot/` – Static files (CSS, JS, images)
  - `Program.cs` – Application start-up and configuration
  - `appsettings.json` – Application configuration

- **Thamco.StaffTests/**  
  Unit and integration tests for the frontend logic.

- **.github/**  
  GitHub-specific configurations and workflows.

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

## Features

- Staff member management via a user interface
- Role and permissions management
- Extensible service structure
- Designed for integration with other OnlineRetailer modules

## DevOps & CI/CD

- GitHub Actions workflows may be configured under `.github/`  
  (Please check the directory for more details.)

## Documentation

- For more details, please refer to code comments and structure.
- This list of files may be incomplete. [View the full directory on GitHub.](https://github.com/Meowmixforme/OnlineRetailer.Staff/tree/main/ThAmCo.Staff)

## Licence

This project is licensed under the terms of the [MIT Licence](LICENSE).
