import axios from 'axios';
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
class ApiClient {
    constructor() {
        Object.defineProperty(this, "client", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.client = axios.create({
            baseURL: API_BASE_URL,
            headers: {
                'Content-Type': 'application/json',
            },
        });
        this.setupInterceptors();
    }
    setupInterceptors() {
        this.client.interceptors.request.use((config) => {
            const token = localStorage.getItem('accessToken');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
            return config;
        });
        this.client.interceptors.response.use((response) => response, async (error) => {
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
                    }
                    catch (refreshError) {
                        localStorage.removeItem('accessToken');
                        localStorage.removeItem('refreshToken');
                        window.location.href = '/login';
                    }
                }
            }
            return Promise.reject(error);
        });
    }
    // Auth endpoints
    register(email, firstName, lastName, password) {
        return this.client.post('/auth/register', {
            email,
            firstName,
            lastName,
            password,
            confirmPassword: password,
        });
    }
    login(email, password) {
        return this.client.post('/auth/login', { email, password });
    }
    refreshToken(refreshToken) {
        return this.client.post('/auth/refresh', { refreshToken });
    }
    getProfile() {
        return this.client.get('/auth/profile');
    }
    // Product endpoints
    getProducts(tenantId, page = 1, pageSize = 10) {
        return this.client.get('/products', {
            params: { tenantId, page, pageSize },
        });
    }
    getProduct(id) {
        return this.client.get(`/products/${id}`);
    }
    createProduct(tenantId, data) {
        return this.client.post('/products', data, {
            params: { tenantId },
        });
    }
    updateProduct(id, data) {
        return this.client.put(`/products/${id}`, data);
    }
    deleteProduct(id) {
        return this.client.delete(`/products/${id}`);
    }
    // Category endpoints
    getCategories(tenantId) {
        return this.client.get('/categories', {
            params: { tenantId },
        });
    }
    getCategory(id) {
        return this.client.get(`/categories/${id}`);
    }
    // Cart endpoints
    getCart() {
        return this.client.get('/cart');
    }
    addToCart(productId, quantity) {
        return this.client.post('/cart/items', { productId, quantity });
    }
    removeFromCart(productId) {
        return this.client.delete(`/cart/items/${productId}`);
    }
    clearCart() {
        return this.client.delete('/cart');
    }
    // Order endpoints
    getOrders(page = 1, pageSize = 10) {
        return this.client.get('/orders', {
            params: { page, pageSize },
        });
    }
    getOrder(id) {
        return this.client.get(`/orders/${id}`);
    }
    createOrder(data) {
        return this.client.post('/orders', data);
    }
    // Review endpoints
    getReviews(productId) {
        return this.client.get(`/reviews?productId=${productId}`);
    }
    createReview(data) {
        return this.client.post('/reviews', data);
    }
    // Theme endpoints
    getTheme(tenantId) {
        return this.client.get(`/themes?tenantId=${tenantId}`);
    }
    updateTheme(id, data) {
        return this.client.put(`/themes/${id}`, data);
    }
}
export default new ApiClient();
