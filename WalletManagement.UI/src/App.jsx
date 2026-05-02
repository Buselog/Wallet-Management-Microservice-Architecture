import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Transaction from './pages/Transaction';
import Investment from './pages/Investment';
import TransactionHistory from './pages/TransactionHistory';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/transaction" element={<Transaction />} />
        <Route path="/investment" element={<Investment />} />
        <Route path="/history" element={<TransactionHistory />} />
        <Route path="/" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}

export default App;