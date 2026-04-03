# Multi-Tenant EcommerceSaaS Platform

A production-ready, scalable multi-tenant ecommerce SaaS platform built with .NET 8, React, TypeScript, and PostgreSQL. Vendors can create and manage their own online stores with customizable themes.

## 🚀 Features

### Core Features
- **Multi-Tenant Architecture**: Complete data isolation with TenantId
- **Store Management**: Create and manage multiple standalone online stores
- **Customizable Themes**: Dynamic theme engine with color customization
- **Role-Based Access Control**: SuperAdmin, Vendor, Customer roles
- **JWT Authentication**: Secure access token + refresh token flow
- **Product Management**: CRUD operations, inventory tracking, categorization
- **Shopping Cart**: Session-based cart management
- **Order Management**: Order placement, tracking, and history
- **Reviews & Ratings**: Customer feedback system
- **Discount System**: Support for coupons and promotional offers
- **Responsive UI**: Mobile-first design with Tailwind CSS

### Security Features (OWASP Compliance)
✅ Input Validation (FluentValidation)
✅ SQL Injection Prevention (EF Core)
✅ XSS Protection (Input Sanitization)
✅ CSRF Protection
✅ Rate Limiting
✅ Secure Headers (HSTS, CSP)
✅ Password Hashing (BCrypt)
✅ JWT Token Verification
✅ File Upload Validation
✅ Audit Logging

## 📋 Table of Contents

1. [Technology Stack](#technology-stack)
2. [Project Structure](#project-structure)
3. [Setup Instructions](#setup-instructions)
4. [API Documentation](#api-documentation)
5. [Database Schema](#database-schema)
6. [Environment Configuration](#environment-configuration)
7. [Deployment](#deployment)

## 🛠 Technology Stack

### Backend
- **.NET 8**: Modern, high-performance web framework
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Reliable, scalable relational database
- **JWT**: Industry-standard authentication
- **FluentValidation**: Fluent validation library
- **Serilog**: Structured logging
- **AutoMapper**: Object mapping

### Frontend
- **React 18**: UI library
- **TypeScript**: Type-safe JavaScript
- **Vite**: Lightning-fast build tool
- **Tailwind CSS**: Utility-first CSS framework
- **Zustand**: Lightweight state management
- **Axios**: HTTP client with interceptors
- **React Router v6**: Client-side routing

## 📁 Project Structure

```
saas/
├── backend/
│   ├── src/
│   │   ├── EcommerceSaaS.Domain/          # Core entities and business logic
│   │   │   ├── Entities/
│   │   │   ├── Enums/
│   │   │   └── ValueObjects/
│   │   ├── EcommerceSaaS.Application/     # Use cases and DTOs
│   │   │   ├── DTOs/
│   │   │   ├── Services/
│   │   │   ├── Validators/
│   │   │   ├── Queries/
│   │   │   └── Commands/
│   │   ├── EcommerceSaaS.Infrastructure/  # Data access and external services
│   │   │   ├── Persistence/
│   │   │   ├── Repositories/
│   │   │   ├── Services/
│   │   │   └── Caching/
│   │   └── EcommerceSaaS.API/             # REST API controllers
│   │       ├── Controllers/
│   │       ├── Middleware/
│   │       ├── Extensions/
│   │       ├── Program.cs
│   │       └── appsettings.json
│   ├── tests/
│   │   └── EcommerceSaaS.Tests.csproj
│   └── EcommerceSaaS.sln
├── frontend/
│   ├── src/
│   │   ├── components/          # Reusable React components
│   │   ├── pages/               # Page components
│   │   ├── services/            # API client
│   │   ├── hooks/               # Custom React hooks
│   │   ├── context/             # Zustand stores
│   │   ├── types/               # TypeScript interfaces
│   │   ├── utils/               # Helper functions
│   │   ├── theme/               # Theme configuration
│   │   ├── App.tsx
│   │   ├── main.tsx
│   │   └── index.css
│   ├── public/
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── tailwind.config.js
│   └── package.json
├── docker/
│   ├── docker-compose.yml       # Local development setup
│   ├── Dockerfile.postgres
│   ├── Dockerfile.api
│   ├── Dockerfile.frontend
│   └── Dockerfile.frontend.prod
└── docs/
    ├── API.md                   # API documentation
    ├── DATABASE.md              # Database schema
    └── SETUP.md                 # Detailed setup guide
```

## ⚙️ Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 13+ (or Docker)
- Git

### Local Development Setup

#### 1. Clone Repository
```bash
git clone https://github.com/yourusername/ecommerce-saas.git
cd saas
```

#### 2. Backend Setup

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Update database (EF Core migrations)
cd src/EcommerceSaaS.API
dotnet ef database update --project ../EcommerceSaaS.Infrastructure

# Run the backend
dotnet run --project src/EcommerceSaaS.API/EcommerceSaaS.API.csproj
```

Backend runs on: `http://localhost:5000`

#### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

Frontend runs on: `http://localhost:3000`

### Docker Setup (Recommended)

```bash
cd docker

# Build and run all services
docker-compose up --build

# Run in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

**Access Points:**
- Frontend: http://localhost:3000
- API: http://localhost:5000/api
- Swagger: http://localhost:5000/swagger
- PostgreSQL: localhost:5432

## 🔌 API Documentation

### Authentication Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "vendor@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "vendor@example.com",
  "password": "SecurePass123!"
}

Response:
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "vendor@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": 2,
      "isActive": true
    }
  }
}
```

#### Get Profile
```http
GET /api/auth/profile
Authorization: Bearer {accessToken}
```

### Product Endpoints

#### Get Products
```http
GET /api/products?tenantId={tenantId}&page=1&pageSize=10

Response:
{
  "success": true,
  "data": {
    "data": [...],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 50,
    "totalPages": 5
  }
}
```

#### Create Product (Vendor Only)
```http
POST /api/products?tenantId={tenantId}
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Amazing Product",
  "slug": "amazing-product",
  "description": "Product description",
  "price": 99.99,
  "discountPrice": 79.99,
  "stockQuantity": 100,
  "sku": "PROD-001"
}
```

#### Update Product
```http
PUT /api/products/{productId}
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "name": "Updated Product Name",
  "price": 89.99,
  "stockQuantity": 50
}
```

#### Delete Product
```http
DELETE /api/products/{productId}
Authorization: Bearer {accessToken}
```

### Cart Endpoints

#### Get Cart
```http
GET /api/cart
Authorization: Bearer {accessToken}
```

#### Add to Cart
```http
POST /api/cart/items
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "productId": "550e8400-e29b-41d4-a716-446655440000",
  "quantity": 2
}
```

#### Remove from Cart
```http
DELETE /api/cart/items/{productId}
Authorization: Bearer {accessToken}
```

### Order Endpoints

#### Create Order
```http
POST /api/orders
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "shippingAddress": "123 Main St, City, State 12345",
  "billingAddress": "123 Main St, City, State 12345",
  "discountCode": "SUMMER20",
  "tax": 15.00,
  "shipping": 10.00
}
```

#### Get Orders
```http
GET /api/orders?page=1&pageSize=10
Authorization: Bearer {accessToken}
```

## 🗄 Database Schema

### Key Tables

**tenants**
- Id (PK)
- TenantId (Unique Multi-Tenant ID)
- Name
- Slug (Unique)
- Domain
- SubscriptionPlan
- Status
- MaxProducts, MaxUsers

**users**
- Id (PK)
- TenantId (FK)
- Email (Unique per Tenant)
- PasswordHash
- Role (1=SuperAdmin, 2=Vendor, 3=Customer)
- IsEmailConfirmed
- RefreshToken

**products**
- Id (PK)
- TenantId (FK) - Data Isolation
- CategoryId (FK)
- Name
- Slug
- Price, DiscountPrice
- StockQuantity
- Status (Draft, Active, OutOfStock, Archived)
- AverageRating

**orders**
- Id (PK)
- TenantId (FK)
- UserId (FK)
- OrderNumber (Unique)
- TotalAmount
- Status (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled)
- PaymentStatus

**cart**
- Id (PK)
- TenantId (FK)
- UserId (FK) - Unique per User per Tenant
- Items (JSON)

**discounts**
- Id (PK)
- TenantId (FK)
- Code (Unique per Tenant)
- Type (Percentage, FixedAmount, FreeShipping)
- Value
- MinOrderAmount
- StartDate, EndDate

## 🔐 Environment Configuration

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ecommerce_saas;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-min-32-characters-long",
    "Issuer": "EcommerceSaaS",
    "Audience": "EcommerceSaaSUsers",
    "ExpirationMinutes": 60,
    "RefreshExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

### Frontend (.env)

```env
REACT_APP_API_URL=http://localhost:5000/api
REACT_APP_ENV=development
```

## 🚀 Deployment

### Production Checklist

- [ ] Update JWT secret in configuration
- [ ] Enable HTTPS
- [ ] Configure CORS for production domain
- [ ] Set up database backups
- [ ] Enable rate limiting
- [ ] Configure email service for notifications
- [ ] Set up monitoring and logging
- [ ] Enable CDN for static assets
- [ ] Test authentication flow
- [ ] Implement payment gateway

### Docker Production Build

```bash
docker-compose -f docker/docker-compose.yml build
docker-compose -f docker/docker-compose.yml push
```

### Kubernetes Deployment (Future)

```bash
kubectl apply -f k8s/namespace.yml
kubectl apply -f k8s/postgres-statefulset.yml
kubectl apply -f k8s/api-deployment.yml
kubectl apply -f k8s/frontend-deployment.yml
```

## 📊 Key Concepts

### Multi-Tenancy
- Each tenant has unique `TenantId`
- Data isolation via `TenantId` column on all data tables
- Tenant resolution via subdomain or header

### Clean Architecture
- **Domain**: Pure business entity
- **Application**: Use cases, DTOs, validators
- **Infrastructure**: Database, external services
- **API**: REST controllers, middleware

### Security
- JWT tokens with expiration
- Bcrypt password hashing
- Input validation on all endpoints
- SQL injection prevention via EF Core
- CSRF protection headers
- Rate limiting middleware
- Secure password policy

## 🧪 Testing

### Unit Tests

```bash
cd backend/tests

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter FullyQualifiedName~AuthServiceTests

# With code coverage
dotnet test /p:CollectCoverage=true
```

## 📝 License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📧 Support

For support, email support@ecommercesaas.com or create an issue in the repository.

---

**Last Updated**: April 3, 2026
