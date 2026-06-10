export default interface AuditLogFilter {
  entityName?: string;
  entityId?: number;
  dateFrom?: string;
  dateTo?: string;
  page?: number;
  pageSize?: number;
}