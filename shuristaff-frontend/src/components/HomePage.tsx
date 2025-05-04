import React from 'react';
import { Link } from 'react-router-dom';
import logo from '../assets/logo.png'; // Adjust path as needed

const HomePage: React.FC = () => {
  return (
    <div> {/* Single parent element */}
      <header className="bg-gray-100 py-4 shadow-md w-screen">
        <div className="mx-auto flex items-center justify-between px-4">
          <Link to="/" className="flex items-center text-xl font-semibold text-gray-800 no-underline">
            <img src={logo} alt="Your Logo" className="h-8 w-8 mr-2 align-middle" /> {/* Adjusted logo styles */}
            <span className="align-middle">Shuriken Discord Bot</span> {/* Aligned text as well */}
          </Link>
          <nav>
            <ul className="list-none p-0 m-0 flex items-center">
              <li className="mr-6">
                <Link to="/" className="text-gray-700 hover:text-blue-500 no-underline align-middle">Home</Link>
              </li>
              <li className="mr-6">
                <Link to="/example-video" className="text-gray-700 hover:text-blue-500 no-underline align-middle">Example Video</Link>
              </li>
              <li className="mr-6">
                <Link to="/login" className="text-gray-700 hover:text-blue-500 no-underline align-middle">Login</Link>
              </li>
            </ul>
          </nav>
        </div>
      </header>
      <main className="container mx-auto py-8 px-4 text-center">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">Welcome to the Home Page!</h1>
        <p className="text-gray-700 leading-relaxed">This is the main content of your website. You can add more information, features, and engaging content here.</p>
        {/* Add more homepage content here */}
      </main>
    </div>
  );
};

export default HomePage;