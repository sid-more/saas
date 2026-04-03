# REST API Documentation

## Base URL

```
Development: http://localhost:5000/api
Production: https://api.ecommercesaas.com/api
```

## Authentication

All endpoints (except public ones) require JWT Bearer token in Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Token obtained from login endpoint. Tokens expire in 1 hour. Use refresh token to obtain new token.

## Response Format

All responses use consistent format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": {},
  "errors": null
}
```

Error responses:
```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Field validation error"]
}
```

## HTTP Status Codes

- `200 OK` - Success
- `201 Created` - Resource created
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

---

## Authentication Endpoints

### POST /auth/register

Register a new user (vendor or customer)

**Request**
```json
{
  "email": "vendor@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character (!@#$%^&*)

**Response** `201 Created`
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "vendor@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": 2,
    "isActive": true
  }
}
```

**Errors**
- Email already registered
- Validation failures

---

### POST /auth/login

Authenticate user and obtain tokens

**Request**
```json
{
  "email": "vendor@example.com",
  "password": "SecurePass123!"
}
```

**Response** `200 OK`
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "vendor@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": 2,
      "isActive": true,
      "lastLogin": "2026-04-03T10:30:00Z"
    }
  }
}
```

**Errors**
- Invalid email or password
- Account inactive

---

### POST /auth/refresh

Refresh access token using refresh token

**Request**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response** `200 OK`
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Errors**
- Invalid or expired refresh token

---

### GET /auth/profile

Get current authenticated user profile

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "vendor@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": 2,
    "isActive": true,
    "lastLogin": "2026-04-03T10:30:00Z"
  }
}
```

---

## Product Endpoints

### GET /products

Get paginated list of products for a tenant

**Query Parameters**
```
tenantId: uuid (required)
page: integer (default: 1)
pageSize: integer (default: 10, max: 100)
```

**Example**
```
GET /api/products?tenantId=550e8400-e29b-41d4-a716-446655440000&page=1&pageSize=20
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": "650e8400-e29b-41d4-a716-446655440001",
        "name": "Wireless Headphones",
        "slug": "wireless-headphones",
        "description": "High-quality wireless headphones",
        "price": 79.99,
        "discountPrice": 59.99,
        "stockQuantity": 50,
        "sku": "ELEC-001",
        "status": 2,
        "categoryId": "750e8400-e29b-41d4-a716-446655440001",
        "categoryName": "Electronics",
        "averageRating": 4.5,
        "images": ["https://..."]
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  }
}
```

---

### GET /products/{id}

Get product details

**Path Parameters**
```
id: uuid (product ID)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "650e8400-e29b-41d4-a716-446655440001",
    "name": "Wireless Headphones",
    "slug": "wireless-headphones",
    "description": "High-quality wireless headphones with noise cancellation",
    "price": 79.99,
    "discountPrice": 59.99,
    "stockQuantity": 50,
    "sku": "ELEC-001",
    "status": 2,
    "categoryId": "750e8400-e29b-41d4-a716-446655440001",
    "categoryName": "Electronics",
    "averageRating": 4.5,
    "images": ["https://..."]
  }
}
```

---

### POST /products

Create new product (Vendor only)

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Query Parameters**
```
tenantId: uuid (required)
```

**Request Body**
```json
{
  "categoryId": "750e8400-e29b-41d4-a716-446655440001",
  "name": "Wireless Headphones",
  "slug": "wireless-headphones",
  "description": "High-quality wireless headphones",
  "price": 79.99,
  "discountPrice": 59.99,
  "stockQuantity": 50,
  "sku": "ELEC-001"
}
```

**Validation Rules**
- name: 3-255 characters
- slug: lowercase, hyphens only, 3-255 characters
- price: > 0
- discountPrice: < price (if provided)
- stockQuantity: >= 0
- sku: 3-100 characters

**Response** `201 Created`
```json
{
  "success": true,
  "message": "Product created successfully",
  "data": {
    "id": "650e8400-e29b-41d4-a716-446655440001",
    "name": "Wireless Headphones",
    "slug": "wireless-headphones",
    ...
  }
}
```

---

### PUT /products/{id}

Update product (Vendor only)

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Request Body** (all fields optional)
```json
{
  "name": "Updated Product Name",
  "description": "Updated description",
  "price": 89.99,
  "discountPrice": 69.99,
  "stockQuantity": 75,
  "status": 2
}
```

**Response** `200 OK`

---

### DELETE /products/{id}

Delete product (soft delete) (Vendor only)

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`
```json
{
  "success": true,
  "message": "Product deleted successfully"
}
```

---

## Cart Endpoints

### GET /cart

Get current user's cart

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "850e8400-e29b-41d4-a716-446655440001",
    "items": [
      {
        "productId": "650e8400-e29b-41d4-a716-446655440001",
        "productName": "Wireless Headphones",
        "quantity": 2,
        "price": 59.99
      }
    ],
    "total": 119.98
  }
}
```

---

### POST /cart/items

Add item to cart

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Request Body**
```json
{
  "productId": "650e8400-e29b-41d4-a716-446655440001",
  "quantity": 2
}
```

**Validation**
- quantity: 1-100

**Response** `200 OK`

---

### DELETE /cart/items/{productId}

Remove item from cart

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`

---

### DELETE /cart

Clear entire cart

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`

---

## Order Endpoints

### GET /orders

Get user's orders

**Headers**
```
Authorization: Bearer {accessToken}
```

**Query Parameters**
```
page: integer (default: 1)
pageSize: integer (default: 10)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": "950e8400-e29b-41d4-a716-446655440001",
        "orderNumber": "ORD-20260403-ABC123DE",
        "totalAmount": 189.99,
        "taxAmount": 15.00,
        "shippingAmount": 10.00,
        "discountAmount": 10.00,
        "status": 2,
        "paymentStatus": 2,
        "createdAt": "2026-04-03T10:30:00Z",
        "shippingAddress": "123 Main St, City, State 12345",
        "items": [
          {
            "productId": "650e8400-e29b-41d4-a716-446655440001",
            "productName": "Wireless Headphones",
            "quantity": 2,
            "unitPrice": 59.99,
            "totalPrice": 119.98
          }
        ]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 5,
    "totalPages": 1
  }
}
```

---

### GET /orders/{id}

Get order details

**Headers**
```
Authorization: Bearer {accessToken}
```

**Response** `200 OK`

---

### POST /orders

Create new order

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Request Body**
```json
{
  "shippingAddress": "123 Main St, City, State 12345",
  "billingAddress": "123 Main St, City, State 12345",
  "discountCode": "SUMMER20",
  "tax": 15.00,
  "shipping": 10.00
}
```

**Response** `201 Created`
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "id": "950e8400-e29b-41d4-a716-446655440001",
    "orderNumber": "ORD-20260403-ABC123DE",
    "totalAmount": 179.99,
    "status": 1,
    "paymentStatus": 1
  }
}
```

---

## Review Endpoints

### GET /reviews

Get product reviews

**Query Parameters**
```
productId: uuid (required)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "a50e8400-e29b-41d4-a716-446655440001",
      "productId": "650e8400-e29b-41d4-a716-446655440001",
      "rating": 5,
      "title": "Great product!",
      "comment": "Excellent quality and fast shipping",
      "authorName": "John D.",
      "isVerifiedPurchase": true,
      "createdAt": "2026-04-01T15:30:00Z"
    }
  ]
}
```

---

### POST /reviews

Create product review

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Request Body**
```json
{
  "productId": "650e8400-e29b-41d4-a716-446655440001",
  "rating": 5,
  "title": "Great product!",
  "comment": "Excellent quality and fast shipping"
}
```

**Validation**
- rating: 1-5
- title: 3-255 characters
- comment: max 2000 characters

**Response** `201 Created`

---

## Discount Endpoints

### GET /discounts

Get active discounts for tenant

**Query Parameters**
```
tenantId: uuid (required)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "b50e8400-e29b-41d4-a716-446655440001",
      "code": "SUMMER20",
      "type": 1,
      "value": 20,
      "minOrderAmount": 50,
      "isActive": true
    }
  ]
}
```

---

## Category Endpoints

### GET /categories

Get product categories for tenant

**Query Parameters**
```
tenantId: uuid (required)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "750e8400-e29b-41d4-a716-446655440001",
      "name": "Electronics",
      "slug": "electronics",
      "description": "Electronic devices and accessories",
      "displayOrder": 1
    }
  ]
}
```

---

## Theme Endpoints

### GET /themes

Get active theme for tenant

**Query Parameters**
```
tenantId: uuid (required)
```

**Response** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "c50e8400-e29b-41d4-a716-446655440001",
    "name": "Modern Blue",
    "primaryColor": "#3B82F6",
    "secondaryColor": "#10B981",
    "logo": "https://...",
    "banner": "https://...",
    "themeJson": {
      "fontFamily": "Segoe UI",
      "fontSize": "16px",
      "layout": "standard"
    },
    "isActive": true
  }
}
```

---

### PUT /themes/{id}

Update theme (Vendor only)

**Headers**
```
Authorization: Bearer {accessToken}
Content-Type: application/json
```

**Request Body**
```json
{
  "name": "Updated Theme",
  "primaryColor": "#FF6B6B",
  "secondaryColor": "#4ECDC4",
  "themeJson": {
    "fontFamily": "Roboto",
    "fontSize": "18px"
  }
}
```

**Response** `200 OK`

---

## Rate Limiting

All endpoints are rate-limited to **100 requests per minute** per IP address.

**Rate Limit Headers**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1680510600
```

When limit exceeded: `429 Too Many Requests`

---

## Error Codes

### Authentication Errors (4xx)

| Code | Message |
|------|---------|
| 400 | Validation failed |
| 401 | Unauthorized |
| 403 | Forbidden (insufficient permissions) |
| 404 | Resource not found |

### Server Errors (5xx)

| Code | Message |
|------|---------|
| 500 | Internal server error |
| 503 | Service unavailable |

---

## Pagination

Paginated endpoints support:
- `page`: Page number (1-indexed)
- `pageSize`: Items per page (1-100)

Response includes:
- `data`: Array of items
- `pageNumber`: Current page
- `pageSize`: Items per page
- `totalCount`: Total items
- `totalPages`: Total pages

---

## Filtering & Sorting

### Product Filtering
- Search by name/description
- Filter by category
- Filter by price range
- Filter by status
- Sort by name, price, rating, newest

*(To be implemented)*

---

## WebSocket Events

*(To be implemented for real-time updates)*

- Order status updates
- Inventory changes
- Price updates

---

## Examples

### Complete Login & Product Browsing Flow

```bash
# 1. Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
  }'

# 2. Login
RESPONSE=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123!"
  }')
TOKEN=$(echo $RESPONSE | jq -r '.data.accessToken')

# 3. Browse products
curl http://localhost:5000/api/products?tenantId=550e8400-e29b-41d4-a716-446655440000

# 4. Add to cart
curl -X POST http://localhost:5000/api/cart/items \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "650e8400-e29b-41d4-a716-446655440001",
    "quantity": 1
  }'

# 5. Create order
curl -X POST http://localhost:5000/api/orders \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "shippingAddress": "123 Main St, City, State 12345",
    "billingAddress": "123 Main St, City, State 12345",
    "tax": 10,
    "shipping": 5
  }'
```

---

**Version**: 1.0
**Last Updated**: April 3, 2026
**Status**: Beta
