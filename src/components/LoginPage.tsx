import React, { useState, ChangeEvent, FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom'; // Import useNavigate for redirection

interface LoginFormState {
  username: string;
  password: string;
  error: string | null;
}

const LoginPage: React.FC = () => {
  const [state, setState] = useState<LoginFormState>({
    username: '',
    password: '',
    error: null,
  });

  const navigate = useNavigate(); // Hook to handle navigation

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setState(prevState => ({
      ...prevState,
      [name]: value,
      error: null,
    }));
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    console.log('Login submitted:', state.username, state.password);
    if (state.username !== 'test' || state.password !== 'password') {
      setState(prevState => ({ ...prevState, error: 'Invalid username or password' }));
    } else {
      console.log('Login successful!');
      // In a real app, you'd likely redirect to the homepage here
      navigate('/dashboard');
    }
  };

  return (
    <div>
      <h2>Login</h2>
      {state.error && <p style={{ color: 'red' }}>{state.error}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            name="username"
            value={state.username}
            onChange={handleChange}
          />
        </div>
        <div>
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            name="password"
            value={state.password}
            onChange={handleChange}
          />
        </div>
        <button type="submit">Log In</button>
      </form>
      <p style={{ marginTop: '10px' }}>
        <Link to="/forgot-password">Forgot Password?</Link>
      </p>
      <button style={{ marginTop: '10px' }}>
        <img src="/discord-logo.png" alt="Login with Discord" style={{ height: '20px', marginRight: '5px', verticalAlign: 'middle' }} />
        Login with Discord
      </button>
      {/* You'll need to handle the Discord login flow separately */}
    </div>
  );
};

export default LoginPage;