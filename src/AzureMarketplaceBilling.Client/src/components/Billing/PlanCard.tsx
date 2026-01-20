import { useTranslation } from 'react-i18next';
import type { Plan } from '../../types';
import { Card, CardHeader, CardBody, CardFooter } from '../Common/Card';
import { Button } from '../Common/Button';

interface PlanCardProps {
  plan: Plan;
  onSelect: (planId: string) => void;
  isCurrentPlan?: boolean;
}

export function PlanCard({ plan, onSelect, isCurrentPlan = false }: PlanCardProps) {
  const { t } = useTranslation();

  return (
    <Card highlighted={plan.isPopular} className="ct-plan-card">
      {plan.isPopular && (
        <div className="ct-plan-card__badge">{t('pricing.popular')}</div>
      )}
      <CardHeader className="ct-plan-card__header">
        <h3 className="ct-plan-card__name">{plan.name}</h3>
        <div className="ct-plan-card__price">
          <span className="ct-plan-card__currency">$</span>
          <span className="ct-plan-card__amount">{plan.monthlyPrice}</span>
          <span className="ct-plan-card__period">/{t('common.month')}</span>
        </div>
        <p className="ct-plan-card__description">{plan.description}</p>
      </CardHeader>
      <CardBody className="ct-plan-card__body">
        <h4 className="ct-plan-card__section-title">{t('pricing.includedCredentials')}</h4>
        <ul className="ct-plan-card__quotas">
          {plan.quotas.map((quota) => (
            <li key={quota.id} className="ct-plan-card__quota-item">
              <span className="ct-plan-card__quota-name">{quota.displayName}</span>
              <span className="ct-plan-card__quota-value">
                {quota.includedQuantity.toLocaleString()}
              </span>
            </li>
          ))}
        </ul>
        <h4 className="ct-plan-card__section-title">{t('pricing.overage')}</h4>
        <ul className="ct-plan-card__overages">
          {plan.quotas.map((quota) => (
            <li key={quota.id} className="ct-plan-card__overage-item">
              <span className="ct-plan-card__overage-name">{quota.displayName}</span>
              <span className="ct-plan-card__overage-price">
                ${quota.overagePrice.toFixed(2)}/{t('common.perUnit')}
              </span>
            </li>
          ))}
        </ul>
      </CardBody>
      <CardFooter className="ct-plan-card__footer">
        <Button
          variant={plan.isPopular ? 'primary' : 'outline'}
          onClick={() => onSelect(plan.id)}
          disabled={isCurrentPlan}
          className="ct-plan-card__button"
        >
          {isCurrentPlan ? t('pricing.currentPlan') : t('pricing.subscribe')}
        </Button>
      </CardFooter>
    </Card>
  );
}
