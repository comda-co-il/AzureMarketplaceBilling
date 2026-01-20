import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import en from './locales/en.json';
import he from './locales/he.json';

const resources = {
  en: { translation: en },
  he: { translation: he }
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: 'he',
    lng: 'he', // Default language is Hebrew
    interpolation: {
      escapeValue: false
    },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage']
    }
  });

export default i18n;

// Helper to check if current language is RTL
export const isRTL = () => {
  return i18n.language === 'he';
};

// Helper to get direction
export const getDirection = () => {
  return isRTL() ? 'rtl' : 'ltr';
};
