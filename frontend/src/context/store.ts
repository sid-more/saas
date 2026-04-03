import { create } from 'zustand';
import { User, AuthResponse } from '@/types';

interface AuthStore {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  setAuth: (data: AuthResponse) => void;
  clearAuth: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthStore>((set) => ({
  user: null,
  accessToken: null,
  refreshToken: null,

  setAuth: (data: AuthResponse) => {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    set({
      user: data.user,
      accessToken: data.accessToken,
      refreshToken: data.refreshToken,
    });
  },

  clearAuth: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    set({
      user: null,
      accessToken: null,
      refreshToken: null,
    });
  },

  isAuthenticated: () => {
    const token = localStorage.getItem('accessToken');
    return !!token;
  },
}));

interface CartStore {
  cartItems: any[];
  addItem: (item: any) => void;
  removeItem: (productId: string) => void;
  clearCart: () => void;
  getTotal: () => number;
}

export const useCartStore = create<CartStore>((set, get) => ({
  cartItems: [],

  addItem: (item: any) => {
    const existing = get().cartItems.find((i) => i.productId === item.productId);
    if (existing) {
      existing.quantity += item.quantity;
      set({ cartItems: [...get().cartItems] });
    } else {
      set({ cartItems: [...get().cartItems, item] });
    }
  },

  removeItem: (productId: string) => {
    set({
      cartItems: get().cartItems.filter((i) => i.productId !== productId),
    });
  },

  clearCart: () => {
    set({ cartItems: [] });
  },

  getTotal: () => {
    return get().cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
  },
}));

interface ThemeStore {
  primaryColor: string;
  secondaryColor: string;
  setPrimaryColor: (color: string) => void;
  setSecondaryColor: (color: string) => void;
}

export const useThemeStore = create<ThemeStore>((set) => ({
  primaryColor: '#3B82F6',
  secondaryColor: '#10B981',

  setPrimaryColor: (color: string) => set({ primaryColor: color }),
  setSecondaryColor: (color: string) => set({ secondaryColor: color }),
}));
