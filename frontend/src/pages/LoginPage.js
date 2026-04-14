import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks';
export default function LoginPage() {
    const navigate = useNavigate();
    const { login, loading, error: apiError } = useAuth();
    const [formData, setFormData] = useState({ email: '', password: '' });
    const [localError, setLocalError] = useState('');
    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };
    const handleSubmit = async (e) => {
        e.preventDefault();
        setLocalError('');
        if (!formData.email || !formData.password) {
            setLocalError('Please fill in all fields');
            return;
        }
        const success = await login(formData.email, formData.password);
        if (success) {
            navigate('/');
        }
    };
    return (_jsx("div", { className: "min-h-screen bg-gradient-to-br from-blue-600 to-blue-800 flex items-center justify-center px-4", children: _jsxs("div", { className: "bg-white rounded-lg shadow-xl p-8 max-w-md w-full", children: [_jsx("h1", { className: "text-3xl font-bold text-gray-900 mb-2 text-center", children: "Welcome Back" }), _jsx("p", { className: "text-gray-600 text-center mb-8", children: "Sign in to your account" }), (localError || apiError) && (_jsx("div", { className: "bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6", children: localError || apiError })), _jsxs("form", { onSubmit: handleSubmit, className: "space-y-4", children: [_jsxs("div", { children: [_jsx("label", { htmlFor: "email", className: "block text-sm font-medium text-gray-700", children: "Email" }), _jsx("input", { type: "email", id: "email", name: "email", value: formData.email, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent", placeholder: "you@example.com" })] }), _jsxs("div", { children: [_jsx("label", { htmlFor: "password", className: "block text-sm font-medium text-gray-700", children: "Password" }), _jsx("input", { type: "password", id: "password", name: "password", value: formData.password, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent", placeholder: "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022" })] }), _jsx("button", { type: "submit", disabled: loading, className: "w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-400 transition font-medium", children: loading ? 'Signing in...' : 'Sign In' })] }), _jsxs("p", { className: "text-center text-gray-600 mt-6", children: ["Don't have an account?", ' ', _jsx("button", { onClick: () => navigate('/register'), className: "text-blue-600 hover:underline font-medium", children: "Register here" })] })] }) }));
}
