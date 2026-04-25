import React, { useEffect, useState } from 'react';
import { requestService } from '../services/api';
import { useAuth } from '../context/AuthContext';
import { Clock, CheckCircle, AlertCircle, Sparkles, Plus, HandCoins, Check } from 'lucide-react';
import { Link } from 'react-router-dom';

const ProviderDashboard = () => {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  const fetchRequests = async () => {
    try {
      // For providers, we fetch all pending requests plus their own accepted requests
      const [allRes, myRes] = await Promise.all([
        requestService.getPending(),
        requestService.getMyRequests()
      ]);
      
      // Combine and remove duplicates by ID
      const combined = [...myRes.data, ...allRes.data.filter(r => !myRes.data.find(m => m.id === r.id))];
      setRequests(combined);
    } catch (err) {
      console.error('Failed to fetch requests', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
    const interval = setInterval(fetchRequests, 10000);
    return () => clearInterval(interval);
  }, []);

  const handleAccept = async (id) => {
    try {
      await requestService.accept(id);
      fetchRequests();
    } catch (err) {
      alert(err.response?.data?.error || 'Failed to accept request');
    }
  };

  const handleComplete = async (id) => {
    try {
      await requestService.complete(id);
      fetchRequests();
    } catch (err) {
      alert(err.response?.data?.error || 'Failed to complete request');
    }
  };

  const getStatusStyle = (status) => {
    switch (status) {
      case 'Pending': return { color: 'var(--warning)', bg: 'rgba(245, 158, 11, 0.1)' };
      case 'Accepted': return { color: 'var(--primary)', bg: 'rgba(99, 102, 241, 0.1)' };
      case 'Completed': return { color: 'var(--success)', bg: 'rgba(16, 185, 129, 0.1)' };
      default: return { color: 'var(--text-muted)', bg: 'rgba(255, 255, 255, 0.1)' };
    }
  };

  return (
    <div className="animate-fade">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
        <h3 style={{ fontSize: '1.4rem' }}>Available Service Requests</h3>
        <span style={{ fontSize: '0.9rem', color: 'var(--text-muted)' }}>Role: Provider</span>
      </div>

      {loading && requests.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '100px', color: 'var(--text-muted)' }}>Searching for opportunities...</div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '20px' }}>
          {requests.map((req) => {
            const statusStyle = getStatusStyle(req.status);
            const isMine = req.providerId === user.id;
            
            return (
              <div key={req.id} className="glass" style={{ padding: '24px', display: 'flex', flexDirection: 'column', gap: '16px', border: isMine ? '1px solid var(--primary)' : '1px solid var(--glass-border)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <div>
                    <h4 style={{ fontSize: '1.2rem', fontWeight: '600' }}>{req.title}</h4>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: 'var(--text-muted)', fontSize: '0.8rem', marginTop: '4px' }}>
                      <Clock size={12} /> {new Date(req.createdAt).toLocaleDateString()}
                    </div>
                  </div>
                  <span style={{ 
                    padding: '4px 12px', 
                    borderRadius: '20px', 
                    fontSize: '0.8rem', 
                    fontWeight: '600',
                    color: statusStyle.color,
                    background: statusStyle.bg,
                    border: `1px solid ${statusStyle.color}44`
                  }}>
                    {req.status}
                  </span>
                </div>

                <p style={{ color: 'var(--text-muted)', fontSize: '0.95rem', flex: 1 }}>
                  {req.aiDescription || req.description}
                </p>

                <div style={{ display: 'flex', gap: '12px', marginTop: '10px' }}>
                  {req.status === 'Pending' && (
                    <button onClick={() => handleAccept(req.id)} className="btn btn-primary" style={{ flex: 1 }}>
                      <HandCoins size={18} /> Accept Job
                    </button>
                  )}
                  {req.status === 'Accepted' && isMine && (
                    <button onClick={() => handleComplete(req.id)} className="btn" style={{ flex: 1, background: 'var(--success)', color: 'white' }}>
                      <Check size={18} /> Mark as Completed
                    </button>
                  )}
                  <button className="btn btn-outline" style={{ flex: 1 }}>
                    View Map
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

export default ProviderDashboard;
