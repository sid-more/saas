import { Link, useNavigate } from 'react-router-dom';
import { ShoppingCartIcon, UserCircleIcon } from '@heroicons/react/24/outline';
import { useAuthStore } from '@/context/store';
import { useCartStore } from '@/context/store';

interface NavbarProps {
  onLogout: () => void;
}

export default function Navbar({ onLogout }: NavbarProps) {
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const cartItems = useCartStore((state) => state.cartItems);

  return (
    <nav className="bg-white shadow-md">
      <div className="container mx-auto px-4">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/" className="text-2xl font-bold text-blue-600">
            EcommerceSaaS
          </Link>

          {/* Navigation Links */}
          <div className="hidden md:flex space-x-8">
            <Link to="/" className="text-gray-600 hover:text-gray-900 transition">
              Products
            </Link>
            {user && user.role === 2 && (
              <Link to="/vendor" className="text-gray-600 hover:text-gray-900 transition">
                Vendor Dashboard
              </Link>
            )}
            {user && user.role === 1 && (
              <Link to="/admin" className="text-gray-600 hover:text-gray-900 transition">
                Admin Panel
              </Link>
            )}
          </div>

          {/* Right Side Actions */}
          <div className="flex items-center space-x-4">
            {/* Cart */}
            <Link to="/cart" className="relative">
              <ShoppingCartIcon className="w-6 h-6 text-gray-600 hover:text-gray-900" />
              {cartItems.length > 0 && (
                <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                  {cartItems.length}
                </span>
              )}
            </Link>

            {/* User Menu */}
            {user ? (
              <div className="flex items-center space-x-3">
                <div className="flex items-center space-x-2 cursor-pointer">
                  <UserCircleIcon className="w-6 h-6 text-gray-600" />
                  <span className="text-gray-700 text-sm">{user.firstName}</span>
                </div>
                <button
                  onClick={onLogout}
                  className="px-4 py-2 text-sm text-white bg-red-500 rounded hover:bg-red-600 transition"
                >
                  Logout
                </button>
              </div>
            ) : (
              <div className="flex space-x-2">
                <button
                  onClick={() => navigate('/login')}
                  className="px-4 py-2 text-sm text-blue-600 border border-blue-600 rounded hover:bg-blue-50 transition"
                >
                  Login
                </button>
                <button
                  onClick={() => navigate('/register')}
                  className="px-4 py-2 text-sm text-white bg-blue-600 rounded hover:bg-blue-700 transition"
                >
                  Register
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
