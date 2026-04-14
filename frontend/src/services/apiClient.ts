import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('accessToken');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    this.client.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          const refreshToken = localStorage.getItem('refreshToken');

          if (refreshToken) {
            try {
              const response = await this.client.post('/auth/refresh', { refreshToken });
              localStorage.setItem('accessToken', response.data.data.accessToken);
              localStorage.setItem('refreshToken', response.data.data.refreshToken);
              return this.client(originalRequest);
            } catch (refreshError) {
              localStorage.removeItem('accessToken');
              localStorage.removeItem('refreshToken');
              window.location.href = '/login';
            }
          }
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  register(email: string, firstName: string, lastName: string, password: string) {
    return this.client.post('/auth/register', {
      email,
      firstName,
      lastName,
      password,
      confirmPassword: password,
    });
  }

  login(email: string, password: string) {
    return this.client.post('/auth/login', { email, password });
  }

  refreshToken(refreshToken: string) {
    return this.client.post('/auth/refresh', { refreshToken });
  }

  getProfile() {
    return this.client.get('/auth/profile');
  }

  // Product endpoints
  getProducts(tenantId: string, page: number = 1, pageSize: number = 10) {
    return this.client.get('/products', {
      params: { tenantId, page, pageSize },
    });
  }

  getProduct(id: string) {
    return this.client.get(`/products/${id}`);
  }

  createProduct(tenantId: string, data: any) {
    return this.client.post('/products', data, {
      params: { tenantId },
    });
  }

  updateProduct(id: string, data: any) {
    return this.client.put(`/products/${id}`, data);
  }

  deleteProduct(id: string) {
    return this.client.delete(`/products/${id}`);
  }

  // Category endpoints
  getCategories(tenantId: string) {
    return this.client.get('/categories', {
      params: { tenantId },
    });
  }

  getCategory(id: string) {
    return this.client.get(`/categories/${id}`);
  }

  // Cart endpoints
  getCart() {
    return this.client.get('/cart');
  }

  addToCart(productId: string, quantity: number) {
    return this.client.post('/cart/items', { productId, quantity });
  }

  removeFromCart(productId: string) {
    return this.client.delete(`/cart/items/${productId}`);
  }

  clearCart() {
    return this.client.delete('/cart');
  }

  // Order endpoints
  getOrders(page: number = 1, pageSize: number = 10) {
    return this.client.get('/orders', {
      params: { page, pageSize },
    });
  }

  getOrder(id: string) {
    return this.client.get(`/orders/${id}`);
  }

  createOrder(data: any) {
    return this.client.post('/orders', data);
  }

  // Review endpoints
  getReviews(productId: string) {
    return this.client.get(`/reviews?productId=${productId}`);
  }

  createReview(data: any) {
    return this.client.post('/reviews', data);
  }

  // Theme endpoints
  getTheme(tenantId: string) {
    return this.client.get(`/themes?tenantId=${tenantId}`);
  }

  updateTheme(id: string, data: any) {
    return this.client.put(`/themes/${id}`, data);
  }
}

export default new ApiClient();
