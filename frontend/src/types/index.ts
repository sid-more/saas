export enum UserRole {
  SuperAdmin = 1,
  Vendor = 2,
  Customer = 3,
}

export enum ProductStatus {
  Draft = 1,
  Active = 2,
  OutOfStock = 3,
  Archived = 4,
}

export enum OrderStatus {
  Pending = 1,
  Confirmed = 2,
  Processing = 3,
  Shipped = 4,
  Delivered = 5,
  Cancelled = 6,
  Refunded = 7,
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  isActive: boolean;
  lastLogin?: Date;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface Product {
  id: string;
  name: string;
  slug: string;
  description?: string;
  price: number;
  discountPrice?: number;
  stockQuantity: number;
  sku: string;
  status: ProductStatus;
  categoryId: string;
  categoryName?: string;
  averageRating: number;
  images: string[];
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  image?: string;
  displayOrder: number;
}

export interface CartItem {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
}

export interface Cart {
  id: string;
  items: CartItem[];
  total: number;
}

export interface OrderItem {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  totalAmount: number;
  taxAmount: number;
  shippingAmount: number;
  discountAmount: number;
  status: OrderStatus;
  paymentStatus: number;
  createdAt: Date;
  shippingAddress?: string;
  items: OrderItem[];
}

export interface Theme {
  id: string;
  name: string;
  primaryColor: string;
  secondaryColor: string;
  logo?: string;
  banner?: string;
  themeJson: string;
  isActive: boolean;
}

export interface Review {
  id: string;
  productId: string;
  rating: number;
  title: string;
  comment?: string;
  authorName?: string;
  isVerifiedPurchase: boolean;
  createdAt: Date;
}
