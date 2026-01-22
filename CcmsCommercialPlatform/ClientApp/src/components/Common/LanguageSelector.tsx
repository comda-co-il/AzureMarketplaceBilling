import React from 'react';
import { useTranslation } from 'react-i18next';

const LanguageSelector: React.FC = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
    // Update document direction
    document.documentElement.dir = lng === 'he' ? 'rtl' : 'ltr';
    document.documentElement.lang = lng;
  };

  return (
    <div className="language-selector">
      <button
        className={`lang-btn ${i18n.language === 'he' ? 'active' : ''}`}
        onClick={() => changeLanguage('he')}
        title="עברית"
      >
        🇮🇱 עב
      </button>
      <button
        className={`lang-btn ${i18n.language === 'en' ? 'active' : ''}`}
        onClick={() => changeLanguage('en')}
        title="English"
      >
        🇺🇸 EN
      </button>
    </div>
  );
};

export default LanguageSelector;
