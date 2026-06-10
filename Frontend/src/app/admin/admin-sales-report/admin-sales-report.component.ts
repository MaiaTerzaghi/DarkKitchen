import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import SalesReportResponse, {
  SalesReportMonth,
} from '../../../backend/services/order/models/SalesReportResponse';

@Component({
  selector: 'app-admin-sales-report',
  templateUrl: './admin-sales-report.component.html',
  standalone: false,
  styleUrls: ['./admin-sales-report.component.css'],
})
export class AdminSalesReportComponent implements OnInit {
  report: SalesReportResponse | null = null;
  loading: boolean = false;
  errorMessage: string = '';

  page: number = 1;
  pageSize: number = 20;

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
    this.loadReport();
  }

  public loadReport(): void {
    this.loading = true;
    this.errorMessage = '';

    this._orderService.getSalesReport(this.page, this.pageSize).subscribe({
      next: (data) => {
        this.report = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar el reporte de ventas';
        this.loading = false;
      },
    });
  }

  public getMonthName(month: number): string {
    const names = [
      'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
      'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre',
    ];
    return names[month - 1] || '';
  }

  public onPageChange(page: number): void {
    this.page = page;
    this.loadReport();
  }

  public formatCurrency(value: number): string {
    return '$' + value.toLocaleString('es-UY', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }
}
