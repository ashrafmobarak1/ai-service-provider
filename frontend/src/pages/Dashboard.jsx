import React from 'react';
import { useAuth } from '../context/AuthContext';
import CustomerDashboard from './CustomerDashboard';
import ProviderDashboard from './ProviderDashboard';

const Dashboard = () => {
  const { user } = useAuth();
  
  console.log('Current User State:', user);

  // Check if user is a provider (Case insensitive check for 'Provider' and 'Admin')
  const isProvider = user?.roles?.some(r => 
    r.toLowerCase() === 'provider' || r.toLowerCase() === 'admin'
  );

  if (isProvider) {
    return <ProviderDashboard />;
  }

  return <CustomerDashboard />;
};

export default Dashboard;
