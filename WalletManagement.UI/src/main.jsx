import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ConfigProvider } from 'antd'
import './i18n';
import './index.css'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ConfigProvider
      theme={{
        token: {
          colorPrimary: '#0F172A',
          colorLink: '#A16207',
          colorSuccess: '#15803D',
          colorWarning: '#B45309',
          colorError: '#DC2626',
          colorInfo: '#2563EB',
          borderRadius: 10,
          fontFamily: '"IBM Plex Sans", ui-sans-serif, system-ui, sans-serif',
        },
      }}
    >
      <App />
    </ConfigProvider>
  </StrictMode>,
)