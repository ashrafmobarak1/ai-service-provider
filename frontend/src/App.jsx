import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import LandingPage from './pages/LandingPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import CustomerDashboard from './pages/CustomerDashboard';
import CreateRequestForm from './pages/CreateRequestForm';
import DashboardLayout from './components/DashboardLayout';
import ProtectedRoute from './components/ProtectedRoute';
import './index.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          
          {/* Protected Dashboard Routes */}
          <Route path="/dashboard" element={
            <ProtectedRoute>
              <DashboardLayout>
                <CustomerDashboard />
              </DashboardLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/dashboard/new-request" element={
            <ProtectedRoute>
              <DashboardLayout>
                <CreateRequestForm />
              </DashboardLayout>
            </ProtectedRoute>
          } />
          
          <Route path="/dashboard/requests" element={
            <ProtectedRoute>
              <DashboardLayout>
                <CustomerDashboard />
              </DashboardLayout>
            </ProtectedRoute>
          } />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
