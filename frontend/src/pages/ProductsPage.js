import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { useEffect, useState } from 'react';
import { useProducts } from '@/hooks';
import { useCartStore } from '@/context/store';
import ProductCard from '@/components/ProductCard';
import { toast } from 'react-toastify';
export default function ProductsPage() {
    const { products, loading, error, fetchProducts, page, setPage, total } = useProducts();
    const [tenantId] = useState('00000000-0000-0000-0000-000000000001');
    const [searchTerm, setSearchTerm] = useState('');
    const addToCart = useCartStore((state) => state.addItem);
    useEffect(() => {
        fetchProducts(tenantId);
    }, [page]);
    const handleAddToCart = (product) => {
        addToCart({
            productId: product.id,
            productName: product.name,
            quantity: 1,
            price: product.discountPrice || product.price,
        });
        toast.success(`${product.name} added to cart!`);
    };
    const filteredProducts = products.filter((p) => p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        p.description?.toLowerCase().includes(searchTerm.toLowerCase()));
    return (_jsxs("div", { className: "py-8", children: [_jsxs("div", { className: "mb-8", children: [_jsx("h1", { className: "text-4xl font-bold text-gray-900 mb-2", children: "Our Products" }), _jsx("p", { className: "text-gray-600", children: "Browse our collection of amazing products" })] }), _jsx("div", { className: "mb-8", children: _jsx("input", { type: "text", placeholder: "Search products...", value: searchTerm, onChange: (e) => setSearchTerm(e.target.value), className: "w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent" }) }), error && (_jsx("div", { className: "bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-6", children: error })), loading ? (_jsx("div", { className: "flex justify-center items-center h-64", children: _jsx("div", { className: "animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" }) })) : filteredProducts.length > 0 ? (_jsxs(_Fragment, { children: [_jsx("div", { className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8", children: filteredProducts.map((product) => (_jsx(ProductCard, { product: product, onAddToCart: handleAddToCart }, product.id))) }), total > 10 && (_jsxs("div", { className: "flex justify-center items-center space-x-2", children: [_jsx("button", { onClick: () => setPage(Math.max(1, page - 1)), disabled: page === 1, className: "px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-50", children: "Previous" }), _jsxs("span", { className: "text-gray-600", children: ["Page ", page, " of ", Math.ceil(total / 10)] }), _jsx("button", { onClick: () => setPage(page + 1), disabled: page * 10 >= total, className: "px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-50", children: "Next" })] }))] })) : (_jsx("div", { className: "text-center py-12", children: _jsx("p", { className: "text-gray-600 text-lg", children: "No products found" }) }))] }));
}
