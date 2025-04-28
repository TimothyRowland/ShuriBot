import React from 'react';

const LoginPage: React.FC = () => {
  const discordClientId = import.meta.env.VITE_DISCORD_CLIENT_ID; // Replace with your Client ID
  const redirectUri = import.meta.env.VITE_DISCORD_REDIRECT_URI; // Replace with your backend callback URL
  const discordAuthUrl = `https://discord.com/oauth2/authorize?client_id=${discordClientId}&redirect_uri=${encodeURIComponent(redirectUri)}&response_type=code&scope=identify%20guilds`;

  const handleDiscordLogin = () => {
    window.location.href = discordAuthUrl;
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
      <h1 className="text-3xl font-bold text-gray-900 mb-6">Log In with Discord</h1>
      <button
        className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
        onClick={handleDiscordLogin}
      >
        Log In with Discord
      </button>
      {/* You might add other login options later */}
    </div>
  );
};

export default LoginPage;