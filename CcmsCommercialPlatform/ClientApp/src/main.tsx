import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import './i18n/config'

// Set initial direction based on default language (English = LTR)
document.documentElement.dir = 'ltr';
document.documentElement.lang = 'en';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
