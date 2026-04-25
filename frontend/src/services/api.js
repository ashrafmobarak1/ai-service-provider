import axios from 'axios';

const API_URL = 'http://localhost:5214/api';

const api = axios.create({
  baseURL: API_URL,
});

// Automatically add the token to requests if it exists
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  login: (email, password) => api.post('/auth/login', { email, password }),
  register: (name, email, password, role) => api.post('/auth/register', { name, email, password, role }),
};

export const requestService = {
  create: (data) => api.post('/requests', data),
  getMyRequests: () => api.get('/requests/my'),
  getPending: () => api.get('/requests/all'),
  getNearby: (params) => api.get('/requests/nearby', { params }),
  accept: (id) => api.put(`/requests/${id}/accept`),
  complete: (id) => api.put(`/requests/${id}/complete`),
  cancel: (id) => api.delete(`/requests/${id}`),
  enhanceDescription: (id) => api.post(`/requests/${id}/enhance`),
};

export default api;
