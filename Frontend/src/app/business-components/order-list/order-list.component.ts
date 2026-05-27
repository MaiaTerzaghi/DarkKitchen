import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import OrderResponse from '../../../backend/services/order/models/OrderResponse';
import OrderFilter from '../../../backend/services/order/models/OrderFilter';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  standalone: false,
  styleUrls: ['./order-list.component.css'],
})
export class OrderListComponent implements OnInit {
  orders: OrderResponse[] = [];
  errorMessage: string = '';
  loading: boolean = false;

  filterDateFrom: string = '';
  filterDateTo: string = '';
  filterStatus: string = '';

  statuses = [
    { value: 'Pending', label: 'Pendiente' },
    { value: 'Prepared', label: 'Preparado' },
    { value: 'OnTheWay', label: 'En camino' },
    { value: 'Delivered', label: 'Entregado' },
    { value: 'NotDelivered', label: 'No entregado' },
    { value: 'Cancelled', label: 'Cancelado' },
    { value: 'Delayed', label: 'Demorado' },
  ];

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  public loadOrders(): void {
    this.loading = true;
    this.errorMessage = '';

    const filters: OrderFilter = {};
    if (this.filterDateFrom) {
      filters.dateFrom = this.filterDateFrom;
    }
    if (this.filterDateTo) {
      filters.dateTo = this.filterDateTo;
    }
    if (this.filterStatus) {
      filters.status = this.filterStatus;
    }

    this._orderService.getClientOrders(filters).subscribe({
      next: (data) => {
        this.orders = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar los pedidos';
        this.loading = false;
      },
    });
  }

  public clearFilters(): void {
    this.filterDateFrom = '';
    this.filterDateTo = '';
    this.filterStatus = '';
    this.loadOrders();
  }

  public statusLabel(status: string): string {
    return this.statuses.find((s) => s.value === status)?.label ?? status;
  }

  public statusClass(status: string): string {
    return 'status-' + status.toLowerCase();
  }
}