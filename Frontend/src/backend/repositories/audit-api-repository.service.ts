import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import AuditLogResponse from '../services/audit/models/AuditLogResponse';
import AuditLogFilter from '../services/audit/models/AuditLogFilter';
import PaginatedResponse from '../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class AuditApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'audit-logs', http);
  }

  public getLogs(filters: AuditLogFilter): Observable<PaginatedResponse<AuditLogResponse>> {
    const params: string[] = [];

    if (filters.entityName) {
      params.push(`EntityName=${filters.entityName}`);
    }
    if (filters.entityId != null) {
      params.push(`EntityId=${filters.entityId}`);
    }
    if (filters.dateFrom) {
      params.push(`DateFrom=${filters.dateFrom}`);
    }
    if (filters.dateTo) {
      params.push(`DateTo=${filters.dateTo}`);
    }
    params.push(`Page=${filters.page ?? 1}`);
    params.push(`PageSize=${filters.pageSize ?? 20}`);

    const query = params.join('&');
    return this.get<PaginatedResponse<AuditLogResponse>>('', query);
  }
}