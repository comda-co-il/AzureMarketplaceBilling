import { useEffect, useState } from 'react';

interface AlertProps {
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  onClose?: () => void;
  autoClose?: boolean;
  duration?: number;
}

export function Alert({
  type,
  message,
  onClose,
  autoClose = true,
  duration = 5000,
}: AlertProps) {
  const [visible, setVisible] = useState(true);

  useEffect(() => {
    if (autoClose) {
      const timer = setTimeout(() => {
        setVisible(false);
        onClose?.();
      }, duration);

      return () => clearTimeout(timer);
    }
  }, [autoClose, duration, onClose]);

  if (!visible) return null;

  const typeClass = type === 'success' ? 'is-confirm' : type === 'error' ? 'is-error' : '';

  return (
    <div className={`ct-alert ${typeClass}`}>
      <span className="ct-alert__message">{message}</span>
      {onClose && (
        <button
          className="ct-alert__close"
          onClick={() => {
            setVisible(false);
            onClose();
          }}
        >
          ×
        </button>
      )}
    </div>
  );
}

// Alert container for managing multiple alerts
interface AlertItem {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
}

interface AlertContainerProps {
  alerts: AlertItem[];
  onDismiss: (id: string) => void;
}

export function AlertContainer({ alerts, onDismiss }: AlertContainerProps) {
  return (
    <div className="ct-alert-container">
      {alerts.map((alert) => (
        <Alert
          key={alert.id}
          type={alert.type}
          message={alert.message}
          onClose={() => onDismiss(alert.id)}
        />
      ))}
    </div>
  );
}
