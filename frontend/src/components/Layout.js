import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/context/store';
import Navbar from './Navbar';
import Footer from './Footer';
export default function Layout() {
    const navigate = useNavigate();
    const { clearAuth } = useAuthStore();
    const handleLogout = () => {
        clearAuth();
        navigate('/login');
    };
    return (_jsxs("div", { className: "flex flex-col min-h-screen", children: [_jsx(Navbar, { onLogout: handleLogout }), _jsx("main", { className: "flex-grow container mx-auto px-4 py-6", children: _jsx(Outlet, {}) }), _jsx(Footer, {})] }));
}
