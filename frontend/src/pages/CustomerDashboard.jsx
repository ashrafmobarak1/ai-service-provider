import React, { useEffect, useState } from 'react';
import { requestService } from '../services/api';
import { Clock, CheckCircle, AlertCircle, Sparkles, Plus } from 'lucide-react';
import { Link } from 'react-router-dom';

const CustomerDashboard = () => {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchRequests = async () => {
    try {
      const response = await requestService.getMyRequests();
      setRequests(response.data);
    } catch (err) {
      console.error('Failed to fetch requests', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
    // Poll for updates every 5 seconds (for AI enhancement status)
    const interval = setInterval(fetchRequests, 5000);
    return () => clearInterval(interval);
  }, []);

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
        <h3 style={{ fontSize: '1.4rem' }}>Your Service Requests</h3>
        <Link to="/dashboard/new-request" className="btn btn-primary">
          <Plus size={18} /> New Request
        </Link>
      </div>

      {loading && requests.length === 0 ? (
        <div style={{ textAlign: 'center', padding: '100px', color: 'var(--text-muted)' }}>Loading requests...</div>
      ) : requests.length === 0 ? (
        <div className="glass" style={{ padding: '60px', textAlign: 'center' }}>
          <AlertCircle size={48} style={{ color: 'var(--text-muted)', marginBottom: '16px' }} />
          <h3>No requests yet</h3>
          <p style={{ color: 'var(--text-muted)', marginBottom: '24px' }}>Start by creating your first service request.</p>
          <Link to="/dashboard/new-request" className="btn btn-primary">Create Request</Link>
        </div>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '20px' }}>
          {requests.map((req) => {
            const statusStyle = getStatusStyle(req.status);
            return (
              <div key={req.id} className="glass" style={{ padding: '24px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <h4 style={{ fontSize: '1.2rem', fontWeight: '600' }}>{req.title}</h4>
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

                {req.status === 'Pending' && !req.aiDescription && (
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--primary)', fontSize: '0.85rem', fontWeight: '500' }}>
                    <Sparkles size={16} className="animate-spin" style={{ animationDuration: '3s' }} />
                    AI is enhancing your description...
                  </div>
                )}

                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '10px', paddingTop: '16px', borderTop: '1px solid var(--glass-border)' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: 'var(--text-muted)', fontSize: '0.85rem' }}>
                    <Clock size={14} /> {new Date(req.createdAt).toLocaleDateString()}
                  </div>
                  <button className="btn btn-outline" style={{ padding: '6px 12px', fontSize: '0.85rem' }}>
                    Details
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

export default CustomerDashboard;
