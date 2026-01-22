import type { ReactNode } from 'react';
import { useEffect } from 'react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  size?: 'small' | 'medium' | 'large';
}

export function Modal({
  isOpen,
  onClose,
  title,
  children,
  size = 'medium',
}: ModalProps) {
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
      document.body.style.overflow = 'hidden';
    }

    return () => {
      document.removeEventListener('keydown', handleEscape);
      document.body.style.overflow = 'unset';
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  return (
    <div className="ct-modal-overlay" onClick={onClose}>
      <div
        className={`ct-modal ct-modal--${size}`}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="ct-modal__header">
          <h2 className="ct-modal__title">{title}</h2>
          <button className="ct-modal__close" onClick={onClose}>
            ×
          </button>
        </div>
        <div className="ct-modal__body">{children}</div>
      </div>
    </div>
  );
}
