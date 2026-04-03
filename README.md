# EcommerceSaaS - Multi-Tenant Ecommerce SaaS Platform

A production-ready, scalable multi-tenant ecommerce SaaS platform built with .NET 8, React, TypeScript, and PostgreSQL.

## 📁 Directory Structure

```
saas/
├── backend/                    # .NET 8 backend
│   ├── src/
│   │   ├── EcommerceSaaS.Domain/       # Core domain entities
│   │   ├── EcommerceSaaS.Application/  # Business logic & DTOs
│   │   ├── EcommerceSaaS.Infrastructure/ # Database & services
│   │   └── EcommerceSaaS.API/          # REST API & controllers
│   ├── tests/                  # Unit tests
│   └── EcommerceSaaS.sln       # Solution file
│
├── frontend/                   # React + TypeScript frontend
│   ├── src/
│   │   ├── components/         # Reusable UI components
│   │   ├── pages/             # Page components
│   │   ├── services/          # API client
│   │   ├── hooks/             # Custom hooks
│   │   ├── context/           # State management (Zustand)
│   │   ├── types/             # TypeScript types
│   │   ├── utils/             # Utilities
│   │   └── theme/             # Tailwind theme
│   ├── package.json           # Dependencies
│   └── vite.config.ts        # Vite configuration
│
├── docker/                     # Docker configuration
│   ├── docker-compose.yml      # Development setup
│   ├── Dockerfile.api
│   ├── Dockerfile.frontend
│   ├── Dockerfile.postgres
│   └── Dockerfile.frontend.prod
│
├── docs/                       # Documentation
│   ├── README.md              # Full documentation
│   ├── QUICKSTART.md          # Quick start guide
│   └── DATABASE.md            # Database schema
│
├── .env.example               # Environment variables template
├── Makefile                   # Build automation
└── docker-compose.local.yml   # Local database only
```

## 🚀 Quick Start

### Docker (Recommended)
```bash
cd docker
docker-compose up -d
```

Access:
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Local Development
```bash
# Terminal 1: Backend
cd backend
dotnet restore
dotnet run --project src/EcommerceSaaS.API/EcommerceSaaS.API.csproj

# Terminal 2: Frontend
cd frontend
npm install
npm run dev
```

## 📖 Documentation

- [Full README](docs/README.md) - Complete documentation
- [Quick Start Guide](docs/QUICKSTART.md) - Get running in 5 minutes
- [Database Schema](docs/DATABASE.md) - Detailed DB documentation
- [API Documentation](docs/API.md) - REST API reference

## 🎯 Key Features

✅ Multi-tenant architecture with complete data isolation
✅ JWT authentication + refresh tokens
✅ Role-based access (SuperAdmin, Vendor, Customer)
✅ Product management with inventory tracking
✅ Shopping cart & order management
✅ Customer reviews & ratings
✅ Discount/coupon system
✅ Customizable themes
✅ OWASP security compliance
✅ Rate limiting & input validation
✅ Responsive UI with Tailwind CSS
✅ Docker support
✅ Comprehensive logging
✅ Unit tests with xUnit

## 🛠 Tech Stack

### Backend
- .NET 8
- EF Core
- PostgreSQL
- JWT Authentication
- FluentValidation
- Serilog

### Frontend
- React 18
- TypeScript
- Vite
- Tailwind CSS
- Zustand
- Axios

## 🔒 Security Features

- ✅ Password hashing (BCrypt)
- ✅ SQL injection prevention (EF Core)
- ✅ XSS protection
- ✅ CSRF protection headers
- ✅ Rate limiting middleware
- ✅ Secure headers (HSTS, CSP)
- ✅ Input validation on all endpoints
- ✅ JWT token verification
- ✅ File upload validation

## 📊 Database

PostgreSQL with multi-tenancy support through `TenantId` column on all tables ensuring complete data isolation per tenant.

Key tables:
- tenants (vendor stores)
- users (all roles)
- products (catalog)
- orders (transactions)
- reviews (feedback)
- discounts (promotions)
- cart (shopping)
- themes (customization)

## 🧪 Testing

```bash
cd backend
dotnet test
```

## 📝 Commands

```bash
# Install dependencies
make install

# Build project
make build

# Run locally
make run-backend
make run-frontend

# Docker
make docker-up
make docker-down

# Database migrations
make migrate

# Tests
make test

# Cleanup
make clean
```

## 📚 API Examples

### Register
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "vendor@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
  }'
```

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "vendor@example.com",
    "password": "SecurePass123!"
  }'
```

### Get Products
```bash
curl http://localhost:5000/api/products?tenantId=00000000-0000-0000-0000-000000000001
```

## 🔄 Architecture

```
┌─────────────────────────────────────────────┐
│ React Frontend (TypeScript + Tailwind)      │
├─────────────────────────────────────────────┤
│ API Client (Axios) | State (Zustand)        │
├─────────────────────────────────────────────┤
│ .NET 8 Web API (Clean Architecture)         │
├─────────────────────────────────────────────┤
│ Application Layer (Services, DTOs)          │
├─────────────────────────────────────────────┤
│ Repository | EF Core | Middleware           │
├─────────────────────────────────────────────┤
│ PostgreSQL Database (Multi-Tenant)          │
└─────────────────────────────────────────────┘
```

## 🚀 Deployment

### Production Checklist
- [ ] Update JWT secret
- [ ] Enable HTTPS
- [ ] Configure CORS
- [ ] Set up database backups
- [ ] Configure email service
- [ ] Enable monitoring
- [ ] Implement payment gateway
- [ ] Set up CDN

### Docker Production
```bash
docker build -t ecommerce-saas-api docker/Dockerfile.api
docker build -t ecommerce-saas-frontend docker/Dockerfile.frontend.prod
```

## 📧 Support

For issues and questions, create a GitHub issue or contact support@ecommercesaas.com

## 📄 License

MIT License - See LICENSE file

---

**Created**: April 2026
**Last Updated**: April 3, 2026
