import { useEffect, useState } from 'react';
import { useAuthStore } from '@/context/store';
import apiClient from '@/services/apiClient';

export const useAuth = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { setAuth, clearAuth, isAuthenticated } = useAuthStore();

  const register = async (email: string, firstName: string, lastName: string, password: string) => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.register(email, firstName, lastName, password);
      return response.data.success;
    } catch (err: any) {
      const message = err.response?.data?.message || 'Registration failed';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const login = async (email: string, password: string) => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.login(email, password);
      if (response.data.success) {
        setAuth(response.data.data);
        return true;
      }
      return false;
    } catch (err: any) {
      const message = err.response?.data?.message || 'Login failed';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    clearAuth();
  };

  const checkAuth = async () => {
    if (isAuthenticated()) {
      try {
        const response = await apiClient.getProfile();
        if (response.data.success) {
          const { user } = response.data.data;
          // Update user in store
          return true;
        }
      } catch (err) {
        clearAuth();
        return false;
      }
    }
    return false;
  };

  useEffect(() => {
    checkAuth();
  }, []);

  return {
    register,
    login,
    logout,
    loading,
    error,
    isAuthenticated,
  };
};

export const useProducts = () => {
  const [products, setProducts] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const fetchProducts = async (tenantId: string) => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.getProducts(tenantId, page, pageSize);
      if (response.data.success) {
        setProducts(response.data.data.data);
        setTotal(response.data.data.totalCount);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch products');
    } finally {
      setLoading(false);
    }
  };

  const getProduct = async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.getProduct(id);
      if (response.data.success) {
        return response.data.data;
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch product');
    } finally {
      setLoading(false);
    }
  };

  return {
    products,
    loading,
    error,
    page,
    setPage,
    total,
    fetchProducts,
    getProduct,
  };
};
