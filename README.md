# 🛒 .NET Microservices E-Commerce Application

A full-stack e-commerce application built using **ASP.NET Core, ASP.NET MVC, Microservices Architecture, Microsoft Azure Service Bus, Entity Framework Core, and SQL Server 2022**.

This project demonstrates the implementation of a modular e-commerce system with independent microservices, asynchronous inter-service communication, secure authentication and authorization, and database operations using Entity Framework Core.

The application follows a microservices-based backend architecture, with an ASP.NET MVC frontend responsible for rendering the user interface and handling client-side application logic.

---

## 📌 Table of Contents

- [Project Overview](#-project-overview)
- [Architecture](#-application-architecture)
- [Technology Stack](#-technology-stack)
- [Microservices](#-microservices)
- [Key Features](#-key-features)
- [Authentication and Authorization](#-authentication-and-authorization)
- [Asynchronous Communication](#-asynchronous-microservices-communication)
- [Database Design and Data Access](#-database-and-data-access)
- [Project Structure](#-project-structure)
- [Application Workflow](#-application-workflow)
- [Prerequisites](#-prerequisites)
- [Installation and Setup](#-installation-and-setup)
- [Configuration](#-configuration)
- [Running the Application](#-running-the-application)
- [Testing APIs](#-testing-apis)
- [Security Considerations](#-security-considerations)
- [Future Enhancements](#-future-enhancements)
- [Author](#-author)

---

## 📖 Project Overview

The **.NET Microservices E-Commerce Application** is designed to demonstrate how a modern e-commerce platform can be developed using Microsoft technologies and a service-oriented architecture.

The application separates business capabilities into individual microservices, such as authentication, product management, coupon management, shopping cart management, and email notifications.

The ASP.NET MVC web application provides the frontend experience and communicates with backend APIs to perform e-commerce operations.

Microsoft Azure Service Bus is used to support asynchronous communication between services, helping reduce direct dependencies between participating components.

The project focuses on practical implementation of:

- ASP.NET MVC application development.
- ASP.NET Core Web API development.
- Microservices architecture and service integration.
- Asynchronous messaging using Microsoft Azure Service Bus.
- JWT-based authentication.
- Role-based authorization using Microsoft Identity.
- Database operations using Entity Framework Core.
- SQL Server 2022 integration.
- RESTful API communication.
- Separation of application responsibilities.

## 🏗️ Application Architecture

The application follows a microservices architecture in which backend functionality is divided into independently organized services.

### High-Level Architecture

```text
                 ┌───────────────────────────┐
                 │       End User             │
                 └─────────────┬─────────────┘
                               │
                               ▼
                 ┌───────────────────────────┐
                 │     ASP.NET MVC           │
                 │     Web Frontend          │
                 │ UI, Views, Client Logic   │
                 └─────────────┬─────────────┘
                               │ HTTP
                               ▼
                 ┌───────────────────────────┐
                 │     ASP.NET Core APIs     │
                 │   API / Service Layer     │
                 └─────────────┬─────────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                    │
          ▼                    ▼                    ▼
 ┌────────────────┐  ┌────────────────┐  ┌────────────────┐
 │ Authentication │  │ Product        │  │ Coupon         │
 │ Service        │  │ Service        │  │ Service        │
 └────────────────┘  └────────────────┘  └────────────────┘
          │                    │                    │
          └────────────────────┼────────────────────┘
                               │
                               ▼
                 ┌───────────────────────────┐
                 │ Microsoft Azure           │
                 │ Service Bus               │
                 │ Asynchronous Messaging    │
                 └─────────────┬─────────────┘
                               │
                 ┌─────────────┴─────────────┐
                 ▼                           ▼
       ┌──────────────────┐       ┌──────────────────┐
       │ Shopping Cart    │       │ Email            │
       │ Service          │       │ Service          │
       └──────────────────┘       └──────────────────┘

       ┌───────────────────────────────────────────┐
       │ Entity Framework Core + SQL Server 2022   │
       │ Persistent Data Storage                   │
       └───────────────────────────────────────────┘
```

*Note: This diagram represents the intended high-level architecture. The exact communication paths, database ownership, and service dependencies depend on the implementation in the source code.*

## 🧰 Technology Stack

| Category | Technologies |
|---|---|
| Frontend | ASP.NET MVC, Razor Views, HTML, CSS, JavaScript |
| Backend | ASP.NET Core, C#, .NET |
| Architecture | Microservices Architecture |
| API Communication | RESTful APIs, HTTP/HTTPS |
| Asynchronous Messaging | Microsoft Azure Service Bus |
| Authentication | JWT (JSON Web Tokens) |
| Authorization | Role-Based Authorization, Microsoft Identity |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server 2022 |
| Development IDE | Microsoft Visual Studio |
| Source Control | Git and GitHub |
| API Testing | Swagger / Postman, where configured |

## 🧩 Microservices

The application contains the following service projects.

### 1. Authentication Service — `Mango.Services.AuthAPI`

Responsible for authentication-related operations and user access management.

Responsibilities may include:

- User registration and login.
- User identity management using Microsoft Identity.
- JWT token generation and validation.
- Role assignment and role-based access control.
- Authentication-related API endpoints.

### 2. Product Service — `Mango.Services.ProductAPI`

Manages product-related information and operations.

Responsibilities include:

- Creating and maintaining product records.
- Retrieving product details.
- Updating product information.
- Deleting products when authorized.
- Performing product database operations through Entity Framework Core.

### 3. Coupon Service — `Mango.Services.CouponAPI`

Provides coupon-related functionality.

Responsibilities include:

- Creating and maintaining coupons.
- Retrieving coupon details.
- Validating coupon information according to application rules.
- Managing discount values and minimum purchase requirements.

### 4. Shopping Cart Service — `Mango.Services.ShoppingCartAPI`

Handles shopping cart operations.

Responsibilities may include:

- Retrieving shopping cart information.
- Adding products to a cart.
- Updating cart items and quantities.
- Removing products from a cart.
- Integrating with other APIs to retrieve product or coupon information.

### 5. Email Service — `Mango.Services.EmailAPI`

Handles email-related operations and supports integration with asynchronous messaging.

Responsibilities may include:

- Receiving email-related messages or requests.
- Processing notification events.
- Sending email notifications through the configured email provider.
- Separating notification processing from the initiating service.

### 6. Web Application — `Mango.Web`

The ASP.NET MVC frontend provides the user-facing interface.

Responsibilities include:

- Rendering application pages using MVC and Razor Views.
- Handling user interactions and form submissions.
- Communicating with backend APIs.
- Displaying product and shopping cart information.
- Managing authentication-related user interactions.
- Implementing frontend validation and client-side logic.

### 7. Message Bus Integration — `Mango.MessageBus`

Contains message bus integration functionality for communication through Microsoft Azure Service Bus.

Depending on the implementation, it can encapsulate messaging operations, such as publishing messages, receiving messages, and supporting communication between services.

---

## ✨ Key Features

### Microservices-Based Backend

- Separates business functionality into dedicated services.
- Organizes APIs by business responsibility.
- Supports modular development and maintenance.
- Reduces the need to implement all business functionality in a single application.

### Asynchronous Inter-Service Communication

- Integrates Microsoft Azure Service Bus for messaging.
- Supports asynchronous communication between participating services.
- Helps decouple message producers from message consumers.
- Allows message processing to occur independently of the initiating request when configured appropriately.

### Secure Authentication

- Uses JWT-based authentication.
- Supports authenticated API access.
- Integrates Microsoft Identity for identity management.
- Enables role-based access restrictions.

### Database Operations

- Uses Entity Framework Core for data access.
- Supports CRUD operations.
- Uses SQL Server 2022 for relational data storage.
- Supports LINQ-based querying and entity tracking where applicable.
- Can use EF Core migrations for schema management if configured.

### MVC Frontend

- Follows the Model-View-Controller pattern.
- Uses Razor Views to render server-side HTML.
- Separates user interface concerns from backend business operations.
- Supports client-side validation and interactive user experiences.

### RESTful APIs

- Exposes HTTP endpoints for application functionality.
- Uses standard HTTP methods such as GET, POST, PUT, and DELETE where appropriate.
- Enables communication between the MVC frontend and backend services.

---

## 🔐 Authentication and Authorization

The application implements JWT authentication and role-based authorization using Microsoft Identity.

### JWT Authentication Flow

1. A user submits login credentials through the application.
2. The authentication service validates the credentials.
3. On successful authentication, the service generates a JWT containing the configured claims.
4. The client or MVC application uses the token according to the implemented authentication flow.
5. Protected APIs validate the token before allowing access to secured endpoints.

### Role-Based Authorization

Role-based authorization restricts access according to a user's assigned role and the authorization rules defined by the application.

For example, a project may use roles such as:

- **Admin:** Access to administrative operations.
- **Customer:** Access to customer-facing operations.

These are illustrative roles; the actual roles depend on the application's configuration.

ASP.NET Core authorization attributes and policies can be used to protect endpoints and application functionality.

Example:

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProduct(int id)
{
    // Implement the product deletion logic.
    return NoContent();
}
```

*This is an illustrative example, not a claim that the exact endpoint exists in the repository.*

---

## 🔄 Asynchronous Microservices Communication

Microsoft Azure Service Bus provides messaging capabilities for communication between services without requiring every interaction to be a synchronous HTTP request.

### Benefits

- **Loose coupling:** Message producers do not need to directly invoke every downstream operation.
- **Asynchronous processing:** Consumers can process messages independently.
- **Resilience:** Queues and retry policies can help handle temporary processing failures when configured.
- **Scalability:** Producers and consumers can be scaled according to workload.
- **Separation of concerns:** Notification and other background tasks can be handled outside the initiating request.

### Typical Messaging Workflow

1. A service performs a business operation.
2. The service publishes a message to Azure Service Bus.
3. The message is delivered to a configured queue or topic.
4. A subscribing service receives and processes the message.
5. The consumer performs the required operation, such as sending an email notification.

The exact message contracts, queue names, topic subscriptions, retry policies, and dead-letter handling depend on the implemented configuration.

---

## 🗄️ Database and Data Access

### Database Technology

**Microsoft SQL Server 2022**

The application uses SQL Server as its relational database and Entity Framework Core as its object-relational mapper.

### Entity Framework Core

Entity Framework Core simplifies database operations by allowing developers to work with C# entities and a database context.

Typical operations include:

- Create records using `AddAsync()`.
- Retrieve records using LINQ and `ToListAsync()`.
- Update entities using `SaveChangesAsync()`.
- Delete records using `Remove()`.
- Manage database schema changes through migrations where configured.

Illustrative example:

```csharp
public async Task<List<Product>> GetProductsAsync()
{
    return await _context.Products.ToListAsync();
}
```

*The entity and database context names above are examples. Actual names may differ in the source code.*

### Database Scripts

The `DatabaseScript` directory can contain SQL scripts required to create database objects, initialize data, or support database setup.

Review the scripts and provide setup instructions for any databases required by the application.

---

## 📁 Project Structure

The repository is organized into multiple projects representing the frontend and backend services.

```text
Mango/
│
├── DatabaseScript/
│   └── SQL database scripts
│
├── Mango.MessageBus/
│   └── Azure Service Bus messaging integration
│
├── Mango.Services.AuthAPI/
│   └── Authentication and authorization API
│
├── Mango.Services.CouponAPI/
│   └── Coupon management API
│
├── Mango.Services.EmailAPI/
│   └── Email and notification API
│
├── Mango.Services.ProductAPI/
│   └── Product management API
│
├── Mango.Services.ShoppingCartAPI/
│   └── Shopping cart API
│
├── Mango.Web/
│   └── ASP.NET MVC frontend
│
├── Mango.sln
│   └── Visual Studio solution
│
├── .gitignore
│
└── README.md
```

The tree above describes the intended organization of the solution. Additional configuration, shared libraries, gateway projects, or infrastructure files may exist in the repository.

---

## ⚙️ Prerequisites

Install and configure the following before running the application:

1. **Visual Studio** with the ASP.NET and web development workload.
2. The .NET SDK version required by the project files.
3. Microsoft SQL Server 2022.
4. SQL Server Management Studio (SSMS), recommended for database management.
5. An Azure subscription and an Azure Service Bus namespace with the required queues or topics.
6. Git for source control.
7. Postman or Swagger UI for API testing, where available.

Verify your .NET installation:

```bash
dotnet --info
```

Check the target framework in each `.csproj` file before installing or selecting the SDK.

---

## 🚀 Installation and Setup

### Step 1: Clone the Repository

```bash
git clone https://github.com/KokateSam/DotNet-Microservices-Ecommerce-Application.git
```

Navigate into the repository:

```bash
cd DotNet-Microservices-Ecommerce-Application
```

### Step 2: Open the Solution

Open `Mango.sln` in Visual Studio.

Restore NuGet packages if Visual Studio does not restore them automatically:

```bash
dotnet restore
```

### Step 3: Configure SQL Server

1. Start your SQL Server 2022 instance.
2. Review the SQL scripts in the `DatabaseScript` directory.
3. Execute the appropriate scripts in SSMS if the project requires manual database initialization.
4. Configure each service's connection string according to its database requirements.
5. Apply EF Core migrations if the project uses migrations and the schema has not already been created.

For example:

```bash
dotnet ef database update
```

Run this command from the appropriate project directory and ensure the required EF Core tooling and design packages are installed.

### Step 4: Configure Azure Service Bus

1. Create or select an Azure Service Bus namespace.
2. Create the required queues or topics and subscriptions.
3. Configure the services to use the appropriate messaging entities.
4. Set the required connection settings using secure configuration.

The configured Azure resources must match the queue and topic names expected by the application.

### Step 5: Configure Authentication

Review the authentication configuration and provide the required settings for JWT generation and validation.

Ensure that the issuer, audience, signing key, token lifetime, and related options are consistent with the application's implementation.

### Step 6: Configure API Endpoints

Review the API base URLs and port settings used by the MVC application.

Make sure that the frontend is configured to communicate with the correct service endpoints in your local development environment.

---

## 🔧 Configuration

Before running the application, verify the configuration settings required by each service.

Typical settings may include:

| Configuration | Purpose |
|---|---|
| SQL Server connection strings | Database connectivity |
| JWT issuer and audience | Token validation |
| JWT signing key | Token signing and verification |
| Azure Service Bus settings | Message publishing and consumption |
| API base URLs | Frontend-to-backend communication |
| Email provider settings | Email delivery |

The actual setting names depend on the application's configuration files and implementation.

**Security recommendation:** Never commit real passwords, JWT signing keys, Azure Service Bus connection strings, email credentials, or production connection strings to GitHub. Use .NET User Secrets for local development and environment variables or an appropriate secret-management service for deployment.

---

## ▶️ Running the Application

### Option 1: Run Using Visual Studio

1. Open `Mango.sln`.
2. Verify that all NuGet packages are restored.
3. Configure SQL Server and Azure Service Bus.
4. Set the required startup projects in Visual Studio.
5. Start the required API services and the MVC frontend.
6. Open the frontend URL shown in the browser or application output.

If multiple services must run simultaneously, configure Visual Studio's multiple startup projects option and select the required services.

### Option 2: Run Individual Projects Using the CLI

From the repository directory, run each service using its project file.

For example:

```bash
dotnet run --project Mango.Services.ProductAPI
```

You can use the same pattern for the other services:

```bash
dotnet run --project Mango.Services.AuthAPI
dotnet run --project Mango.Services.CouponAPI
dotnet run --project Mango.Services.EmailAPI
dotnet run --project Mango.Services.ShoppingCartAPI
dotnet run --project Mango.Web
```

Run these commands in separate terminals when the services need to execute concurrently. If the project files are nested inside subdirectories, adjust the paths accordingly.

The application may require additional services or configuration before it starts successfully.

---

## 🧪 Testing APIs

Use Swagger UI, if configured, or Postman to test the backend APIs.

Suggested test scenarios:

- Verify that authentication endpoints respond correctly.
- Register or authenticate a user.
- Obtain a JWT and access protected endpoints.
- Verify that unauthorized requests are rejected.
- Verify that role-restricted endpoints enforce authorization.
- Test product CRUD operations.
- Test coupon retrieval and management operations.
- Test shopping cart operations.
- Verify asynchronous message publishing and consumption.
- Confirm that email notifications are processed as expected.
- Verify that database changes persist in SQL Server.

Document the actual endpoint URLs and sample request bodies once they have been verified in the running application.

---

## 🔒 Security Considerations

This project demonstrates several security-related implementation areas:

- JWT-based authentication.
- Role-based authorization.
- Identity management using Microsoft Identity.
- Secured API endpoints.
- Secure configuration of database and messaging credentials.

For production deployments, additional measures should be considered:

- Enforce HTTPS.
- Store secrets outside source control.
- Use least-privilege database and Azure identities.
- Validate incoming request data.
- Apply authorization checks on sensitive operations.
- Configure token expiry and signing-key rotation.
- Use appropriate Azure Service Bus access policies.
- Implement logging, monitoring, and error handling without exposing sensitive information.

---

## 🔮 Future Enhancements

Potential improvements for future versions include:

- Docker containerization for each microservice.
- Docker Compose for local multi-service orchestration.
- Kubernetes deployment.
- Azure App Service or Azure Container Apps deployment.
- CI/CD pipelines using GitHub Actions or Azure DevOps.
- API gateway configuration.
- Centralized logging and distributed tracing.
- Automated unit and integration testing.
- Health checks and service monitoring.
- Improved retry policies and dead-letter message handling.
- Distributed transaction strategies and the outbox pattern where required.

These are possible enhancements and should not be interpreted as features already implemented.

---

## 👨‍💻 Author

**Samadhan Kokate**

Full Stack .NET Developer | ASP.NET Core | C# | ASP.NET MVC | Angular | Entity Framework Core | SQL Server | Microservices | Azure

GitHub: [KokateSam](https://github.com/KokateSam)

Project Repository: [DotNet-Microservices-Ecommerce-Application](https://github.com/KokateSam/DotNet-Microservices-Ecommerce-Application)

---

## ⭐ Support

If you find this project useful for learning about .NET development, microservices, asynchronous messaging, or ASP.NET MVC, consider giving the repository a star on GitHub.

Thank you for visiting the project!
