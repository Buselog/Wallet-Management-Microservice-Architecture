import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import MainLayout from './components/MainLayout';
import Dashboard from './pages/Dashboard';
import Transaction from './pages/Transaction';
import Investment from './pages/Investment';
import TransactionHistory from './pages/TransactionHistory';
import Help from './pages/Help';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route path="/dashboard" element={<MainLayout><Dashboard /></MainLayout>} />
        <Route path="/transaction" element={<MainLayout><Transaction /></MainLayout>} />
        <Route path="/investment" element={<MainLayout><Investment /></MainLayout>} />
        <Route path="/history" element={<MainLayout><TransactionHistory /></MainLayout>} />
        <Route path="/help" element={<MainLayout><Help /></MainLayout>} />
        <Route path="/" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}

export default App;