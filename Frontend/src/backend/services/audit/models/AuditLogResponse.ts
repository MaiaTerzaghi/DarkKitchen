export default interface AuditLogResponse {
  id: number;
  timestamp: string;
  entityName: string;
  entityId: number;
  description: string;
  responsibleUser: string;
}