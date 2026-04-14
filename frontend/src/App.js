import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from '@/context/store';
import LoginPage from '@/pages/LoginPage';
import RegisterPage from '@/pages/RegisterPage';
import ProductsPage from '@/pages/ProductsPage';
import ProductDetailPage from '@/pages/ProductDetailPage';
import CartPage from '@/pages/CartPage';
import CheckoutPage from '@/pages/CheckoutPage';
import OrdersPage from '@/pages/OrdersPage';
import VendorDashboard from '@/pages/VendorDashboard';
import AdminDashboard from '@/pages/AdminDashboard';
import NotFound from '@/pages/NotFound';
import Layout from '@/components/Layout';
const ProtectedRoute = ({ element }) => {
    const { isAuthenticated } = useAuthStore();
    if (!isAuthenticated()) {
        return _jsx(Navigate, { to: "/login", replace: true });
    }
    return element;
};
export default function App() {
    return (_jsx(BrowserRouter, { children: _jsxs(Routes, { children: [_jsx(Route, { path: "/login", element: _jsx(LoginPage, {}) }), _jsx(Route, { path: "/register", element: _jsx(RegisterPage, {}) }), _jsxs(Route, { element: _jsx(ProtectedRoute, { element: _jsx(Layout, {}) }), children: [_jsx(Route, { path: "/", element: _jsx(ProductsPage, {}) }), _jsx(Route, { path: "/products/:id", element: _jsx(ProductDetailPage, {}) }), _jsx(Route, { path: "/cart", element: _jsx(CartPage, {}) }), _jsx(Route, { path: "/checkout", element: _jsx(CheckoutPage, {}) }), _jsx(Route, { path: "/orders", element: _jsx(OrdersPage, {}) }), _jsx(Route, { path: "/vendor/*", element: _jsx(VendorDashboard, {}) }), _jsx(Route, { path: "/admin/*", element: _jsx(AdminDashboard, {}) })] }), _jsx(Route, { path: "*", element: _jsx(NotFound, {}) })] }) }));
}
