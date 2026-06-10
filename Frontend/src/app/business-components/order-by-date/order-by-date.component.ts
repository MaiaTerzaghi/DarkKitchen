import { Component } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import OrderResponse from '../../../backend/services/order/models/OrderResponse';

@Component({
  selector: 'app-order-by-date',
  templateUrl: './order-by-date.component.html',
  standalone: false,
  styleUrls: ['./order-by-date.component.css'],
})
export class OrderByDateComponent {
  orders: OrderResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  searched: boolean = false;

  // Filtros
  filterDateFrom: string = '';
  filterDateTo: string = '';
  filterStreet: string = '';
  filterStatus: string = '';

  // Opciones de estado
  statusOptions = [
    { value: 0, label: 'Pendiente' },
    { value: 1, label: 'Preparado' },
    { value: 2, label: 'Cancelado' },
    { value: 3, label: 'En camino' },
    { value: 4, label: 'Entregado' },
    { value: 5, label: 'No entregado' },
    { value: 6, label: 'Demorado' },
  ];

  constructor(private readonly _orderService: OrderService) {}

  public search(): void {
    if (!this.filterDateFrom || !this.filterDateTo) {
      this.errorMessage = 'Los campos Desde y Hasta son obligatorios.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const filters: any = {
      dateFrom: this.filterDateFrom,
      dateTo: this.filterDateTo,
    };

    if (this.filterStreet) {
      filters.street = this.filterStreet;
    }
    if (this.filterStatus !== '') {
      filters.status = Number(this.filterStatus);
    }

    this._orderService.getOrders(filters).subscribe({
      next: (data) => {
        this.orders = data;
        this.searched = true;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al consultar pedidos';
        this.loading = false;
      },
    });
  }

  public clearFilters(): void {
    this.filterDateFrom = '';
    this.filterDateTo = '';
    this.filterStreet = '';
    this.filterStatus = '';
    this.orders = [];
    this.searched = false;
    this.errorMessage = '';
  }

  public getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Pending: 'Pendiente',
      Prepared: 'Preparado',
      Cancelled: 'Cancelado',
      OnTheWay: 'En camino',
      Delivered: 'Entregado',
      NotDelivered: 'No entregado',
      Delayed: 'Demorado',
    };
    return map[status] || status;
  }

  public getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Pending: 'status-pending',
      Prepared: 'status-prepared',
      Cancelled: 'status-cancelled',
      OnTheWay: 'status-ontheway',
      Delivered: 'status-delivered',
      NotDelivered: 'status-notdelivered',
      Delayed: 'status-delayed',
    };
    return map[status] || '';
  }
}
