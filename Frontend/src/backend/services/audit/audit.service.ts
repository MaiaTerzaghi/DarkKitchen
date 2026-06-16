import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuditApiRepositoryService } from '../../repositories/audit-api-repository.service';
import AuditLogResponse from './models/AuditLogResponse';
import AuditLogFilter from './models/AuditLogFilter';
import PaginatedResponse from '../../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class AuditService {
  constructor(private readonly _repository: AuditApiRepositoryService) {}

  public getLogs(filters: AuditLogFilter): Observable<PaginatedResponse<AuditLogResponse>> {
    return this._repository.getLogs(filters);
  }
}