import React from 'react';
import { Link } from 'react-router-dom';
import { Sparkles, Shield, MapPin, Zap } from 'lucide-react';

const LandingPage = () => {
  return (
    <div className="animate-fade">
      <nav className="container" style={{ padding: '20px 0', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1 style={{ fontSize: '1.5rem' }}>AI <span className="gradient-text">Marketplace</span></h1>
        <div style={{ display: 'flex', gap: '20px' }}>
          <Link to="/login" className="btn btn-outline">Login</Link>
          <Link to="/register" className="btn btn-primary">Join Now</Link>
        </div>
      </nav>

      <main className="container" style={{ marginTop: '100px', textAlign: 'center' }}>
        <div style={{ maxWidth: '800px', margin: '0 auto' }}>
          <h2 style={{ fontSize: '4rem', lineHeight: '1.1', marginBottom: '24px' }}>
            Find the Best Services, Enhanced by <span className="gradient-text">AI</span>
          </h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '1.2rem', marginBottom: '40px' }}>
            A premium marketplace connecting customers with professional providers. 
            Smart descriptions, secure payments, and real-time tracking.
          </p>
          <div style={{ display: 'flex', gap: '16px', justifyContent: 'center' }}>
            <Link to="/register" className="btn btn-primary" style={{ padding: '16px 32px', fontSize: '1.1rem' }}>
              Get Started <Zap size={20} />
            </Link>
            <button className="btn btn-outline" style={{ padding: '16px 32px', fontSize: '1.1rem' }}>
              Learn More
            </button>
          </div>
        </div>

        <section style={{ marginTop: '120px', display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '30px' }}>
          <div className="glass" style={{ padding: '30px', textAlign: 'left' }}>
            <div style={{ background: 'rgba(99, 102, 241, 0.2)', width: '48px', height: '48px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '20px' }}>
              <Sparkles className="gradient-text" />
            </div>
            <h3>AI Descriptions</h3>
            <p style={{ color: 'var(--text-muted)', marginTop: '10px' }}>
              We use AI to polish your service requests, making them more attractive to providers.
            </p>
          </div>

          <div className="glass" style={{ padding: '30px', textAlign: 'left' }}>
            <div style={{ background: 'rgba(244, 63, 94, 0.2)', width: '48px', height: '48px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '20px' }}>
              <MapPin style={{ color: 'var(--accent)' }} />
            </div>
            <h3>Local Discovery</h3>
            <p style={{ color: 'var(--text-muted)', marginTop: '10px' }}>
              Find providers right in your neighborhood with our advanced geolocation system.
            </p>
          </div>

          <div className="glass" style={{ padding: '30px', textAlign: 'left' }}>
            <div style={{ background: 'rgba(16, 185, 129, 0.2)', width: '48px', height: '48px', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '20px' }}>
              <Shield style={{ color: 'var(--success)' }} />
            </div>
            <h3>Secure Roles</h3>
            <p style={{ color: 'var(--text-muted)', marginTop: '10px' }}>
              Advanced RBAC system ensuring that your data and transactions are always safe.
            </p>
          </div>
        </section>
      </main>
    </div>
  );
};

export default LandingPage;
