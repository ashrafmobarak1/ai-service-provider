# Implementation Plan - React Frontend (Premium UI)

Build a high-end, responsive React application to consume the .NET API.

## Design Aesthetic
- **Theme**: Dark Mode with subtle gradients.
- **Glassmorphism**: Use `backdrop-filter: blur()` for cards and navigation.
- **Typography**: Outfit or Inter (Google Fonts).
- **Animations**: CSS transitions and subtle hover effects.

## Proposed Components

### 1. Layout & Navigation
- **Navbar**: Sticky, transparent blurred background.
- **Sidebar**: For the dashboard views (Customer/Provider).

### 2. Auth Flow
- **Login/Register**: Modern centered cards with animated backgrounds.

### 3. Service Request Management
- **Request Feed**: Card-based layout with status badges.
- **Create Request Modal**: Step-by-step form with map integration (simulated or Leaflet).
- **AI Enhancement View**: Real-time status updates for Hangfire jobs.

## Technical Stack
- **Framework**: React (Vite).
- **Icons**: Lucide React.
- **Styling**: Vanilla CSS (Custom properties).
- **State Management**: React Context (Simple) or just local state for MVP.

## Verification Plan
- **Mock Testing**: Run without backend first if needed.
- **API Integration**: Connect to `http://localhost:5214`.

### Manual Verification
- Test login flow.
- Create a request and verify AI enhancement status reflects the background job.
