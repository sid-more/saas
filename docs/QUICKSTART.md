# Quick Start Guide

## Prerequisites
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 13+ OR Docker
- Git

## Option 1: Docker (Recommended for Quick Start)

### Start Everything
```bash
cd docker
docker-compose up -d
```

Wait 10-15 seconds for services to initialize.

**Access Points:**
- Frontend: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Stop Services
```bash
docker-compose down
```

### Logs
```bash
docker-compose logs -f api
docker-compose logs -f frontend
docker-compose logs -f postgres
```

## Option 2: Local Development

### 1. Backend Setup

```bash
# Navigate to backend
cd backend

# Restore packages
dotnet restore

# Apply migrations to create database
cd src/EcommerceSaaS.API
dotnet ef database update --project ../EcommerceSaaS.Infrastructure

# Run API
dotnet run --project .
```

Backend starts at: http://localhost:5000

### 2. Frontend Setup

```bash
# In a new terminal, navigate to frontend
cd frontend

# Install dependencies
npm install

# Start dev server
npm run dev
```

Frontend starts at: http://localhost:3000

## Testing the API

### 1. Register a User
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
  }'
```

### 2. Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SecurePass123!"
  }'
```

Copy the `accessToken` from response.

### 3. Get Products
```bash
curl http://localhost:5000/api/products?tenantId=00000000-0000-0000-0000-000000000001
```

### 4. Create Product (Vendor)
```bash
curl -X POST http://localhost:5000/api/products?tenantId=00000000-0000-0000-0000-000000000001 \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "categoryId": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Test Product",
    "slug": "test-product",
    "price": 29.99,
    "stockQuantity": 100,
    "sku": "PROD-001"
  }'
```

## Frontend Features

### Available Pages
- `/` - Product listing
- `/login` - Login page
- `/register` - Registration page
- `/cart` - Shopping cart
- `/checkout` - Checkout (under development)
- `/orders` - Order history (under development)
- `/vendor` - Vendor dashboard (under development)
- `/admin` - Admin panel (under development)

### Default Test Account
- Email: test@example.com
- Password: SecurePass123!

## Environment Variables

### Backend (.env or appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ecommerce_saas;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "your-secret-key-at-least-32-chars",
    "Issuer": "EcommerceSaaS",
    "Audience": "EcommerceSaaSUsers"
  }
}
```

### Frontend (.env.local)
```
REACT_APP_API_URL=http://localhost:5000/api
```

## Troubleshooting

### Database Connection Error
- Ensure PostgreSQL is running: `docker ps | grep postgres`
- Check connection string in appsettings.json
- Verify database exists: `psql -U postgres -l`

### Port Already in Use
- Backend (5000): `lsof -i :5000` then `kill PID`
- Frontend (3000): `lsof -i :3000` then `kill PID`
- Database (5432): `lsof -i :5432` then `kill PID`

### JWT Token Expired
- Get a new token by logging out and logging back in
- Tokens expire after 1 hour
- Use refresh token to get new access token

### CORS Errors
- Check `Cors:AllowedOrigins` in appsettings.json
- Ensure frontend URL is included
- Default: http://localhost:3000

## Next Steps

1. ✅ Run the application
2. 📚 Read [API Documentation](./API.md)
3. 🗄️ Understand [Database Schema](./DATABASE.md)
4. 🛠️ Build vendor dashboard
5. 🔐 Implement payment integration
6. 📊 Add analytics
7. 🚀 Deploy to production

## Need Help?

- Check logs: `docker-compose logs -f`
- Read error messages carefully
- Create GitHub issue for bugs
- Contact support@ecommercesaas.com
