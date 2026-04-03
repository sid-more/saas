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

  return (
    <div className="flex flex-col min-h-screen">
      <Navbar onLogout={handleLogout} />
      <main className="flex-grow container mx-auto px-4 py-6">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
