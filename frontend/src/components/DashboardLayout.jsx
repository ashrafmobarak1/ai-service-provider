import React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { 
  LayoutDashboard, 
  PlusCircle, 
  ClipboardList, 
  Settings, 
  LogOut, 
  User as UserIcon,
  Bell
} from 'lucide-react';

const DashboardLayout = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const menuItems = [
    { icon: <LayoutDashboard size={20} />, label: 'Dashboard', path: '/dashboard' },
    { icon: <PlusCircle size={20} />, label: 'New Request', path: '/dashboard/new-request' },
    { icon: <ClipboardList size={20} />, label: 'My Requests', path: '/dashboard/requests' },
  ];

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--bg-dark)' }}>
      {/* Sidebar */}
      <aside className="glass" style={{ width: '260px', margin: '20px', borderRadius: '24px', display: 'flex', flexDirection: 'column', padding: '30px 20px' }}>
        <div style={{ marginBottom: '40px', paddingLeft: '10px' }}>
          <h1 style={{ fontSize: '1.2rem' }}>AI <span className="gradient-text">Market</span></h1>
        </div>

        <nav style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '8px' }}>
          {menuItems.map((item) => (
            <Link 
              key={item.path} 
              to={item.path} 
              className={`btn ${location.pathname === item.path ? 'btn-primary' : 'btn-outline'}`}
              style={{ justifyContent: 'flex-start', border: 'none', background: location.pathname === item.path ? 'var(--primary)' : 'transparent' }}
            >
              {item.icon} {item.label}
            </Link>
          ))}
        </nav>

        <div style={{ marginTop: 'auto', display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <button onClick={handleLogout} className="btn btn-outline" style={{ justifyContent: 'flex-start', color: 'var(--accent)', border: 'none' }}>
            <LogOut size={20} /> Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main style={{ flex: 1, padding: '40px 40px 40px 0', display: 'flex', flexDirection: 'column', gap: '30px' }}>
        <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h2 style={{ fontSize: '1.8rem' }}>Hello, {user?.name} 👋</h2>
            <p style={{ color: 'var(--text-muted)' }}>Welcome back to your dashboard</p>
          </div>
          <div style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
            <button className="glass" style={{ width: '40px', height: '40px', display: 'flex', alignItems: 'center', justifyContent: 'center', borderRadius: '12px' }}>
              <Bell size={20} />
            </button>
            <div className="glass" style={{ display: 'flex', alignItems: 'center', gap: '12px', padding: '6px 16px', borderRadius: '12px' }}>
              <div style={{ width: '32px', height: '32px', background: 'var(--primary)', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <UserIcon size={18} color="white" />
              </div>
              <span style={{ fontWeight: '500' }}>{user?.subscription} Plan</span>
            </div>
          </div>
        </header>

        <section style={{ flex: 1 }}>
          {children}
        </section>
      </main>
    </div>
  );
};

export default DashboardLayout;
