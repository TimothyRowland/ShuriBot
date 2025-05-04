import React from 'react';

const UnauthorizedPage: React.FC = () => {
  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
      <h1 className="text-3xl font-bold text-red-500 mb-4">Unauthorized</h1>
      <p className="text-gray-700">You do not have the necessary permissions to access this area.</p>
      {/* You might add a link back to the login page here */}
    </div>
  );
};

export default UnauthorizedPage;