import type { DimensionUsage } from '../../types';

interface UsageBarProps {
  dimension: DimensionUsage;
  showDetails?: boolean;
}

export function UsageBar({ dimension, showDetails = true }: UsageBarProps) {
  const percentage = Math.min(dimension.usagePercentage, 100);
  const overagePercentage = dimension.usagePercentage > 100 
    ? Math.min(dimension.usagePercentage - 100, 50) 
    : 0;

  const getStatusClass = (status: string) => {
    switch (status) {
      case 'Critical':
        return 'ct-usage-bar--critical';
      case 'Warning':
        return 'ct-usage-bar--warning';
      default:
        return 'ct-usage-bar--normal';
    }
  };

  return (
    <div className={`ct-usage-bar ${getStatusClass(dimension.status)}`}>
      <div className="ct-usage-bar__header">
        <span className="ct-usage-bar__name">{dimension.displayName}</span>
        <span className="ct-usage-bar__stats">
          {dimension.usedQuantity.toLocaleString()} / {dimension.includedQuantity.toLocaleString()}
          {dimension.overageQuantity > 0 && (
            <span className="ct-usage-bar__overage">
              {' '}(+{dimension.overageQuantity.toLocaleString()} overage)
            </span>
          )}
        </span>
      </div>
      <div className="ct-usage-bar__track">
        <div
          className="ct-usage-bar__fill"
          style={{ width: `${percentage}%` }}
        />
        {overagePercentage > 0 && (
          <div
            className="ct-usage-bar__overage-fill"
            style={{ 
              left: '100%',
              width: `${overagePercentage}%` 
            }}
          />
        )}
      </div>
      {showDetails && (
        <div className="ct-usage-bar__details">
          <span className="ct-usage-bar__percentage">
            {dimension.usagePercentage.toFixed(1)}%
          </span>
          {dimension.overageCharges > 0 && (
            <span className="ct-usage-bar__charges">
              Overage Charges: ${dimension.overageCharges.toFixed(2)}
            </span>
          )}
        </div>
      )}
    </div>
  );
}
