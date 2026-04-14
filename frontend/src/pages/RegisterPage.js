import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks';
export default function RegisterPage() {
    const navigate = useNavigate();
    const { register, loading, error: apiError } = useAuth();
    const [formData, setFormData] = useState({
        email: '',
        firstName: '',
        lastName: '',
        password: '',
        confirmPassword: '',
    });
    const [formError, setFormError] = useState('');
    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };
    const handleSubmit = async (e) => {
        e.preventDefault();
        setFormError('');
        if (!formData.email || !formData.firstName || !formData.lastName || !formData.password) {
            setFormError('Please fill in all fields');
            return;
        }
        if (formData.password !== formData.confirmPassword) {
            setFormError('Passwords do not match');
            return;
        }
        const success = await register(formData.email, formData.firstName, formData.lastName, formData.password);
        if (success) {
            navigate('/login');
        }
    };
    return (_jsx("div", { className: "min-h-screen bg-gradient-to-br from-green-600 to-green-800 flex items-center justify-center px-4", children: _jsxs("div", { className: "bg-white rounded-lg shadow-xl p-8 max-w-md w-full", children: [_jsx("h1", { className: "text-3xl font-bold text-gray-900 mb-2 text-center", children: "Create Account" }), _jsx("p", { className: "text-gray-600 text-center mb-8", children: "Join EcommerceSaaS today" }), (formError || apiError) && (_jsx("div", { className: "bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6", children: formError || apiError })), _jsxs("form", { onSubmit: handleSubmit, className: "space-y-4", children: [_jsxs("div", { className: "grid grid-cols-2 gap-4", children: [_jsxs("div", { children: [_jsx("label", { htmlFor: "firstName", className: "block text-sm font-medium text-gray-700", children: "First Name" }), _jsx("input", { type: "text", id: "firstName", name: "firstName", value: formData.firstName, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent" })] }), _jsxs("div", { children: [_jsx("label", { htmlFor: "lastName", className: "block text-sm font-medium text-gray-700", children: "Last Name" }), _jsx("input", { type: "text", id: "lastName", name: "lastName", value: formData.lastName, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent" })] })] }), _jsxs("div", { children: [_jsx("label", { htmlFor: "email", className: "block text-sm font-medium text-gray-700", children: "Email" }), _jsx("input", { type: "email", id: "email", name: "email", value: formData.email, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent" })] }), _jsxs("div", { children: [_jsx("label", { htmlFor: "password", className: "block text-sm font-medium text-gray-700", children: "Password" }), _jsx("input", { type: "password", id: "password", name: "password", value: formData.password, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent" })] }), _jsxs("div", { children: [_jsx("label", { htmlFor: "confirmPassword", className: "block text-sm font-medium text-gray-700", children: "Confirm Password" }), _jsx("input", { type: "password", id: "confirmPassword", name: "confirmPassword", value: formData.confirmPassword, onChange: handleChange, className: "mt-1 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent" })] }), _jsx("button", { type: "submit", disabled: loading, className: "w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 disabled:bg-gray-400 transition font-medium", children: loading ? 'Creating Account...' : 'Create Account' })] }), _jsxs("p", { className: "text-center text-gray-600 mt-6", children: ["Already have an account?", ' ', _jsx("button", { onClick: () => navigate('/login'), className: "text-green-600 hover:underline font-medium", children: "Sign in here" })] })] }) }));
}
