import React from 'react';
import { useAuth } from '../context/AuthContext';
import CustomerDashboard from './CustomerDashboard';
import ProviderDashboard from './ProviderDashboard';

const Dashboard = () => {
  const { user } = useAuth();
  
  // Check if user is a provider
  const isProvider = user?.roles?.includes('Provider') || user?.roles?.includes('SystemAdmin');

  if (isProvider) {
    return <ProviderDashboard />;
  }

  return <CustomerDashboard />;
};

export default Dashboard;
