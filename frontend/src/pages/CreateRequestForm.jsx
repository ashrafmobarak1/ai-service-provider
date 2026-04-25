import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { requestService } from '../services/api';
import { Send, MapPin, AlignLeft, Type, Loader2 } from 'lucide-react';

const CreateRequestForm = () => {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    address: '',
    latitude: 0,
    longitude: 0
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await requestService.create(formData);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.error || 'Failed to create request. Check your subscription limits.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="animate-fade" style={{ maxWidth: '600px' }}>
      <h3 style={{ fontSize: '1.4rem', marginBottom: '24px' }}>Create New Service Request</h3>

      <div className="glass" style={{ padding: '30px' }}>
        {error && (
          <div style={{ background: 'rgba(244, 63, 94, 0.1)', color: 'var(--accent)', padding: '12px', borderRadius: '8px', marginBottom: '20px', fontSize: '0.9rem', border: '1px solid rgba(244, 63, 94, 0.2)' }}>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <label style={{ display: 'block', marginBottom: '8px', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Title</label>
          <div style={{ position: 'relative' }}>
            <Type size={18} style={{ position: 'absolute', left: '12px', top: '14px', color: 'var(--text-muted)' }} />
            <input
              type="text"
              placeholder="e.g. Need help with plumbing"
              required
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              style={{ paddingLeft: '44px' }}
            />
          </div>

          <label style={{ display: 'block', marginBottom: '8px', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Description</label>
          <div style={{ position: 'relative' }}>
            <AlignLeft size={18} style={{ position: 'absolute', left: '12px', top: '14px', color: 'var(--text-muted)' }} />
            <textarea
              placeholder="Describe your problem in detail. Our AI will help professionalize it later."
              required
              rows={4}
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              style={{ paddingLeft: '44px', minHeight: '120px' }}
            />
          </div>

          <label style={{ display: 'block', marginBottom: '8px', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Address</label>
          <div style={{ position: 'relative' }}>
            <MapPin size={18} style={{ position: 'absolute', left: '12px', top: '14px', color: 'var(--text-muted)' }} />
            <input
              type="text"
              placeholder="Street name, City, Country"
              required
              value={formData.address}
              onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              style={{ paddingLeft: '44px' }}
            />
          </div>

          <div style={{ display: 'flex', gap: '16px', marginTop: '20px' }}>
            <button type="button" onClick={() => navigate('/dashboard')} className="btn btn-outline" style={{ flex: 1 }}>
              Cancel
            </button>
            <button type="submit" className="btn btn-primary" style={{ flex: 2 }} disabled={loading}>
              {loading ? <Loader2 className="animate-spin" /> : <><Send size={18} /> Submit Request</>}
            </button>
          </div>
        </form>
      </div>

      <div style={{ marginTop: '24px', padding: '20px', background: 'rgba(99, 102, 241, 0.1)', borderRadius: '12px', display: 'flex', gap: '16px', alignItems: 'center' }}>
        <div style={{ background: 'var(--primary)', padding: '10px', borderRadius: '10px' }}>
          <Loader2 size={24} color="white" />
        </div>
        <div>
          <h4 style={{ fontSize: '0.95rem' }}>AI Enhancement Included</h4>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>Your description will be automatically rewritten by Claude AI for better provider engagement.</p>
        </div>
      </div>
    </div>
  );
};

export default CreateRequestForm;
