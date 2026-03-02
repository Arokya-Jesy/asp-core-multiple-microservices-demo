# ASP.NET Core 10 Microservices Demo

A complete microservices architecture with API Gateway, authentication, and React frontend.

## Architecture

```
React Client (Port 3000)
    ↓
API Gateway (Port 5000) - Ocelot
    ↓
    ├─→ Auth Service (Port 5001) - JWT Authentication
    ├─→ Product Service (Port 5002) - CRUD Operations
    └─→ Order Service (Port 5003) - Order Management
         ↓                              ↓
    SQL Server (Port 1433)    ──→ Product Service (Inter-service)
```

## Prerequisites

- Docker Desktop installed
- Ports 1433, 3000, 5000, 5001, 5002, 5003 available

## Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd asp-core-10-multiple-microservice-demo
```

### 2. Start All Services
```bash
docker-compose up --build -d
```

This will start:
- SQL Server database
- Auth microservice
- Product microservice
- Order microservice
- API Gateway
- React client

### 3. Wait for Services to Start
```bash
docker-compose ps
```

All services should show "Up" status.

### 4. Access the Application

**React Client:** http://localhost:3000

**API Gateway:** http://localhost:5000
- Auth endpoints: http://localhost:5000/auth/*
- Product endpoints: http://localhost:5000/products/*
- Order endpoints: http://localhost:5000/orders/*

**Swagger Documentation:**
- Auth Service: http://localhost:5001/swagger
- Product Service: http://localhost:5002/swagger
- Order Service: http://localhost:5003/swagger

### 5. Login

Default credentials:
- **Username:** `admin`
- **Password:** `admin123`
- **Role:** admin (full CRUD access)

## Features

### Authentication Service
- User registration and login
- JWT token generation
- BCrypt password hashing
- Role-based access (admin/user)

### Product Service
- List all products
- Create product (admin only)
- Update product (admin only)
- Delete product (admin only)

### Order Service
- Create order (authenticated users)
- View user's order history
- Inter-service communication with Product service
- Fetches product details when creating orders

### API Gateway
- Single entry point for all services
- Request routing with Ocelot
- CORS enabled

### React Client
- Login page
- Product management with CRUD
- Order management (create orders, view history)
- Role-based UI (admin sees edit/delete buttons)
- JWT token management

### Global Features
- Exception handling middleware
- log4net logging (console + file)
- Swagger UI for API testing

## Development

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-service
docker-compose logs -f product-service
docker-compose logs -f order-service
```

### Stop Services
```bash
docker-compose down
```

### Rebuild After Code Changes
```bash
docker-compose up --build -d
```

## API Endpoints

### Authentication (via Gateway)
```
POST /auth/login
POST /auth/register
```

### Products (via Gateway)
```
GET    /products
GET    /products/{id}
POST   /products       (admin only)
PUT    /products/{id}  (admin only)
DELETE /products/{id}  (admin only)
```

### Orders (via Gateway)
```
GET    /orders         (user's orders)
POST   /orders         (create order)
```

## Technology Stack

- **Backend:** ASP.NET Core 10.0
- **Database:** SQL Server 2022
- **API Gateway:** Ocelot
- **Authentication:** JWT Bearer
- **Logging:** log4net
- **Frontend:** React 18 + Vite
- **Containerization:** Docker

## Project Structure

```
├── AuthMicroservice/          # Authentication service
├── ProductMicroservice/       # Product CRUD service
├── OrderMicroservice/         # Order service with inter-service calls
├── ApiGateway/                # Ocelot API Gateway
├── SharedExtensions/          # Shared JWT & middleware
├── client/                    # React frontend
└── docker-compose.yml         # Docker orchestration
```

## Troubleshooting

### Services not starting?
```bash
docker-compose down
docker-compose up --build -d
```

### Port already in use?
Change ports in `docker-compose.yml`

### Database connection issues?
Wait 30 seconds for SQL Server to fully start, then restart services:
```bash
docker-compose restart auth-service product-service order-service
```