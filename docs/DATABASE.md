# Database Schema Documentation

## Overview
PostgreSQL database supporting multi-tenant ecommerce platform with complete data isolation.

## ER Diagram

```
┌─────────────┐
│   Tenant    │ (Multi-Tenant Owner)
└─────────────┘
      │
      ├─── 1:Many ──→ ┌──────────┐
      │               │  User    │ (All roles)
      │               └──────────┘
      │
      ├─── 1:Many ──→ ┌──────────┐
      │               │  Product │
      │               └──────────┘
      │                    │
      │                    ├─── 1:Many ──→ ┌──────────┐
      │                    │               │ Review   │
      │                    │               └──────────┘
      │                    │
      │                    └─── 1:Many ──→ ┌──────────┐
      │                                    │ CartItem │
      │                                    └──────────┘
      │
      ├─── 1:Many ──→ ┌──────────┐
      │               │  Order   │
      │               └──────────┘
      │                    │
      │                    └─── 1:Many ──→ ┌──────────┐
      │                                    │OrderItem │
      │                                    └──────────┘
      │
      ├─── 1:Many ──→ ┌──────────┐
      │               │ Category │
      │               └──────────┘
      │
      ├─── 1:Many ──→ ┌──────────┐
      │               │  Theme   │
      │               └──────────┘
      │
      └─── 1:Many ──→ ┌──────────┐
                      │ Discount │
                      └──────────┘
```

## Table Specifications

### 1. Tenants Table
```sql
CREATE TABLE tenants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL UNIQUE,
    domain VARCHAR(255),
    logo VARCHAR(500),
    subscription_plan INT NOT NULL DEFAULT 1,  -- 1=Freemium, 2=Starter, 3=Pro, 4=Enterprise
    status INT NOT NULL DEFAULT 1,               -- 1=Active, 2=Inactive, 3=Suspended, 4=Deleted
    max_products INT NOT NULL,
    max_users INT NOT NULL,
    subscription_expiry TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    INDEX idx_tenant_id (tenant_id),
    INDEX idx_slug (slug),
    INDEX idx_domain (domain)
);
```

**Purpose**: Represents each vendor/store in the SaaS platform

---

### 2. Users Table
```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role INT NOT NULL,  -- 1=SuperAdmin, 2=Vendor, 3=Customer
    is_email_confirmed BOOLEAN DEFAULT FALSE,
    refresh_token VARCHAR(500),
    refresh_token_expiry TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    last_login TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, email),
    INDEX idx_tenant_email (tenant_id, email),
    INDEX idx_role (tenant_id, role),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id)
);
```

**Multi-Tenancy**: Each user belongs to specific tenant

---

### 3. Products Table
```sql
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    category_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(18, 2) NOT NULL,
    discount_price DECIMAL(18, 2),
    stock_quantity INT NOT NULL DEFAULT 0,
    sku VARCHAR(100) NOT NULL,
    status INT NOT NULL DEFAULT 1,  -- 1=Draft, 2=Active, 3=OutOfStock, 4=Archived
    view_count INT DEFAULT 0,
    average_rating DECIMAL(3, 2) DEFAULT 0,
    images TEXT,  -- Comma-separated or JSON
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, slug),
    INDEX idx_tenant_slug (tenant_id, slug),
    INDEX idx_tenant_status (tenant_id, status),
    INDEX idx_category (category_id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id),
    FOREIGN KEY (category_id) REFERENCES categories(id)
);
```

**Data Isolation**: Products isolated by tenant_id

---

### 4. Categories Table
```sql
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL,
    description VARCHAR(1000),
    image VARCHAR(500),
    parent_category_id UUID,
    display_order INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, slug),
    INDEX idx_tenant_slug (tenant_id, slug),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id),
    FOREIGN KEY (parent_category_id) REFERENCES categories(id)
);
```

---

### 5. Cart Table
```sql
CREATE TABLE carts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    items JSONB,  -- Array of CartItem objects
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, user_id),
    INDEX idx_tenant_user (tenant_id, user_id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

**Structure**:
```json
{
  "items": [
    {
      "productId": "uuid",
      "productName": "string",
      "quantity": 2,
      "price": 29.99
    }
  ]
}
```

---

### 6. Orders Table
```sql
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    order_number VARCHAR(50) NOT NULL,
    status INT NOT NULL DEFAULT 1,  -- 1=Pending, 2=Confirmed, 3=Processing, 4=Shipped, 5=Delivered, 6=Cancelled, 7=Refunded
    payment_status INT NOT NULL DEFAULT 1,  -- 1=Pending, 2=Completed, 3=Failed, 4=Refunded
    total_amount DECIMAL(18, 2) NOT NULL,
    tax_amount DECIMAL(18, 2) DEFAULT 0,
    shipping_amount DECIMAL(18, 2) DEFAULT 0,
    discount_amount DECIMAL(18, 2) DEFAULT 0,
    shipped_date TIMESTAMP,
    delivered_date TIMESTAMP,
    shipping_address VARCHAR(500),
    billing_address VARCHAR(500),
    notes VARCHAR(1000),
    items JSONB,  -- Array of OrderItem objects
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, order_number),
    INDEX idx_tenant_number (tenant_id, order_number),
    INDEX idx_tenant_status (tenant_id, status),
    INDEX idx_user_orders (user_id, tenant_id),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

**OrderItem Structure**:
```json
{
  "items": [
    {
      "productId": "uuid",
      "productName": "string",
      "quantity": 2,
      "unitPrice": 29.99,
      "totalPrice": 59.98
    }
  ]
}
```

---

### 7. Reviews Table
```sql
CREATE TABLE reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    product_id UUID NOT NULL,
    user_id UUID NOT NULL,
    rating INT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    title VARCHAR(255) NOT NULL,
    comment VARCHAR(2000),
    is_verified_purchase BOOLEAN DEFAULT FALSE,
    helpful_count INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(product_id, user_id),  -- One review per product per user
    INDEX idx_tenant_product (tenant_id, product_id),
    INDEX idx_product_rating (product_id, rating),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id),
    FOREIGN KEY (product_id) REFERENCES products(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

---

### 8. Discounts Table
```sql
CREATE TABLE discounts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    code VARCHAR(50) NOT NULL,
    description VARCHAR(500),
    type INT NOT NULL,  -- 1=Percentage, 2=FixedAmount, 3=FreeShipping
    value DECIMAL(18, 2) NOT NULL,
    max_discount DECIMAL(18, 2),
    min_order_amount DECIMAL(18, 2) DEFAULT 0,
    max_uses INT NOT NULL DEFAULT -1,  -- -1 for unlimited
    used_count INT DEFAULT 0,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    UNIQUE(tenant_id, code),
    INDEX idx_tenant_code (tenant_id, code),
    INDEX idx_active_dates (tenant_id, is_active, start_date, end_date),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id)
);
```

---

### 9. Themes Table
```sql
CREATE TABLE themes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    primary_color CHAR(7) NOT NULL DEFAULT '#000000',  -- Hex color
    secondary_color CHAR(7) NOT NULL DEFAULT '#FFFFFF',
    logo VARCHAR(500),
    banner VARCHAR(500),
    theme_json JSONB,  -- Custom theme configuration
    is_active BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    
    INDEX idx_tenant_active (tenant_id, is_active),
    FOREIGN KEY (tenant_id) REFERENCES tenants(tenant_id)
);
```

**ThemeJson Structure**:
```json
{
  "fontFamily": "Segoe UI",
  "fontSize": "16px",
  "layout": "standard",
  "headerStyle": "modern",
  "footerStyle": "dark",
  "customCSS": ""
}
```

---

## Indexes for Performance

```sql
-- Multi-tenant queries
CREATE INDEX idx_products_tenant_active ON products(tenant_id, status) WHERE NOT is_deleted;
CREATE INDEX idx_orders_tenant ON orders(tenant_id, created_at DESC) WHERE NOT is_deleted;
CREATE INDEX idx_users_tenant ON users(tenant_id, role) WHERE NOT is_deleted;

-- User queries
CREATE INDEX idx_orders_user ON orders(user_id) WHERE NOT is_deleted;
CREATE INDEX idx_reviews_user ON reviews(user_id) WHERE NOT is_deleted;

-- Search queries
CREATE INDEX idx_products_search ON products USING GIN (to_tsvector('english', name || ' ' || COALESCE(description, ''))) WHERE NOT is_deleted;
```

## Data Isolation Strategy

All queries filter by `tenant_id`:

```sql
-- Example: Get products for tenant
SELECT * FROM products 
WHERE tenant_id = $1 AND is_deleted = FALSE
ORDER BY created_at DESC;

-- Example: Get orders for user in tenant
SELECT * FROM orders 
WHERE tenant_id = $1 AND user_id = $2 AND is_deleted = FALSE
ORDER BY created_at DESC;
```

## Soft Deletes

All tables have `is_deleted` boolean column. Deleted records are:
- Not shown in queries (filtered out by default)
- Retained for audit purposes
- Can be recovered if needed

## Migration Notes

For EF Core migrations:

```bash
# Create new migration
dotnet ef migrations add MigrationName --project ../EcommerceSaaS.Infrastructure

# Apply migration
dotnet ef database update --project ../EcommerceSaaS.Infrastructure

# Revert last migration
dotnet ef migrations remove
```

## Performance Considerations

- Use connection pooling for database connections
- Consider denormalizing for frequently accessed data
- Archive old orders to separate table periodically
- Monitor slow queries with `pg_stat_statements`
- Update statistics regularly: `ANALYZE`
