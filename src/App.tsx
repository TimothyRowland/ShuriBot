import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './components/HomePage';
import LoginPage from './components/LoginPage';
import ExampleVideoPage from './components/ExampleVideoPage';
import DashboardPage from './components/DashboardPage'; // Import the new component

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/example-video" element={<ExampleVideoPage />} />
        <Route path="/dashboard" element={<DashboardPage />} /> {/* Add the dashboard route */}
        {/* Add a route for the forgot password page later */}
      </Routes>
    </Router>
  );
}

export default App;