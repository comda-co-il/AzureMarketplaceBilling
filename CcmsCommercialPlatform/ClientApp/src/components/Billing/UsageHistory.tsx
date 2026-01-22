import { useState, useEffect } from 'react';
import type { UsageRecord } from '../../types';
import { TokenUsageType, TokenUsageTypeNames } from '../../types';
import { Table } from '../Common/Table';
import { Button } from '../Common/Button';
import { usageApi } from '../../services/api';

interface UsageHistoryProps {
  subscriptionId: string;
}

export function UsageHistory({ subscriptionId }: UsageHistoryProps) {
  const [records, setRecords] = useState<UsageRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<{
    startDate?: string;
    endDate?: string;
    dimensionType?: TokenUsageType;
  }>({});

  useEffect(() => {
    loadHistory();
  }, [subscriptionId, filter]);

  const loadHistory = async () => {
    try {
      setLoading(true);
      const data = await usageApi.getHistory(subscriptionId, {
        startDate: filter.startDate,
        endDate: filter.endDate,
        dimensionType: filter.dimensionType,
      });
      setRecords(data);
    } catch (error) {
      console.error('Failed to load usage history:', error);
    } finally {
      setLoading(false);
    }
  };

  const exportToCsv = () => {
    const headers = ['Date', 'Dimension', 'Used Quantity', 'Reported Overage'];
    const rows = records.map((record) => [
      new Date(record.lastUpdated).toLocaleDateString(),
      TokenUsageTypeNames[record.dimensionType],
      record.usedQuantity.toString(),
      record.reportedOverage.toString(),
    ]);

    const csvContent = [
      headers.join(','),
      ...rows.map((row) => row.join(',')),
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `usage-history-${subscriptionId}.csv`;
    link.click();
  };

  const columns = [
    {
      key: 'lastUpdated',
      header: 'Date',
      render: (record: UsageRecord) =>
        new Date(record.lastUpdated).toLocaleDateString(),
    },
    {
      key: 'dimensionType',
      header: 'Dimension',
      render: (record: UsageRecord) =>
        TokenUsageTypeNames[record.dimensionType],
    },
    {
      key: 'usedQuantity',
      header: 'Used Quantity',
      render: (record: UsageRecord) =>
        record.usedQuantity.toLocaleString(),
    },
    {
      key: 'reportedOverage',
      header: 'Reported Overage',
      render: (record: UsageRecord) =>
        record.reportedOverage.toLocaleString(),
    },
    {
      key: 'billingPeriodStart',
      header: 'Billing Period',
      render: (record: UsageRecord) =>
        new Date(record.billingPeriodStart).toLocaleDateString(),
    },
  ];

  return (
    <div className="ct-usage-history">
      <div className="ct-usage-history__filters">
        <div className="ct-input-group">
          <label className="ct-label--primary">Start Date</label>
          <input
            type="date"
            className="ct-input--lined"
            value={filter.startDate || ''}
            onChange={(e) =>
              setFilter({ ...filter, startDate: e.target.value || undefined })
            }
          />
        </div>
        <div className="ct-input-group">
          <label className="ct-label--primary">End Date</label>
          <input
            type="date"
            className="ct-input--lined"
            value={filter.endDate || ''}
            onChange={(e) =>
              setFilter({ ...filter, endDate: e.target.value || undefined })
            }
          />
        </div>
        <div className="ct-input-group">
          <label className="ct-label--primary">Dimension</label>
          <select
            className="ct-input--lined"
            value={filter.dimensionType ?? ''}
            onChange={(e) =>
              setFilter({
                ...filter,
                dimensionType: e.target.value
                  ? (Number(e.target.value) as TokenUsageType)
                  : undefined,
              })
            }
          >
            <option value="">All Dimensions</option>
            {Object.entries(TokenUsageTypeNames).map(([value, name]) => (
              <option key={value} value={value}>
                {name}
              </option>
            ))}
          </select>
        </div>
        <Button variant="outline" onClick={exportToCsv}>
          Export CSV
        </Button>
      </div>
      <Table
        columns={columns}
        data={records}
        keyExtractor={(record) => record.id}
        loading={loading}
        emptyMessage="No usage history found"
      />
    </div>
  );
}
