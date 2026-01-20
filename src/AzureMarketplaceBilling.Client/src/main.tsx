import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import './i18n/config'

// Set initial direction based on default language (Hebrew = RTL)
document.documentElement.dir = 'rtl';
document.documentElement.lang = 'he';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
