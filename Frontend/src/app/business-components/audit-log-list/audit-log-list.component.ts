import { Component } from '@angular/core';
import { AuditService } from '../../../backend/services/audit/audit.service';
import AuditLogResponse from '../../../backend/services/audit/models/AuditLogResponse';

@Component({
  selector: 'app-audit-log-list',
  templateUrl: './audit-log-list.component.html',
  standalone: false,
  styleUrls: ['./audit-log-list.component.css'],
})
export class AuditLogListComponent {
  logs: AuditLogResponse[] = [];
  loading: boolean = false;
  searched: boolean = false;
  errorMessage: string = '';
  validationError: string = '';

  filterEntityName: string = '';
  filterEntityId: number | null = null;
  filterDateFrom: string = '';
  filterDateTo: string = '';

  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;

  entities = [
    { value: 'Product', label: 'Producto' },
    { value: 'Promotion', label: 'Promoción' },
  ];

  constructor(private readonly _auditService: AuditService) {}

  public search(): void {
    this.validationError = '';

    if (!this.filterEntityName) {
      this.validationError = 'La entidad es obligatoria.';
      return;
    }
    if (this.filterEntityId == null) {
      this.validationError = 'El id de entidad es obligatorio.';
      return;
    }
    if (!this.filterDateFrom) {
      this.validationError = 'La fecha-hora desde es obligatoria.';
      return;
    }
    if (!this.filterDateTo) {
      this.validationError = 'La fecha-hora hasta es obligatoria.';
      return;
    }
    if (this.filterDateFrom >= this.filterDateTo) {
      this.validationError = 'La fecha-hora desde debe ser menor que la fecha-hora hasta.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this._auditService
      .getLogs({
        entityName: this.filterEntityName,
        entityId: this.filterEntityId,
        dateFrom: this.filterDateFrom,
        dateTo: this.filterDateTo,
        page: this.currentPage,
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (response) => {
          this.logs = response.items;
          this.totalCount = response.totalCount;
          this.searched = true;
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage = err || 'Error al consultar la auditoría';
          this.loading = false;
        },
      });
  }

  public onPageChange(page: number): void {
    this.currentPage = page;
    this.search();
  }

  public entityLabel(entityName: string): string {
    return this.entities.find((e) => e.value === entityName)?.label ?? entityName;
  }
}