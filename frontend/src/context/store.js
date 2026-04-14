import { create } from 'zustand';
export const useAuthStore = create((set) => ({
    user: null,
    accessToken: null,
    refreshToken: null,
    setAuth: (data) => {
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
export const useCartStore = create((set, get) => ({
    cartItems: [],
    addItem: (item) => {
        const existing = get().cartItems.find((i) => i.productId === item.productId);
        if (existing) {
            existing.quantity += item.quantity;
            set({ cartItems: [...get().cartItems] });
        }
        else {
            set({ cartItems: [...get().cartItems, item] });
        }
    },
    removeItem: (productId) => {
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
export const useThemeStore = create((set) => ({
    primaryColor: '#3B82F6',
    secondaryColor: '#10B981',
    setPrimaryColor: (color) => set({ primaryColor: color }),
    setSecondaryColor: (color) => set({ secondaryColor: color }),
}));
