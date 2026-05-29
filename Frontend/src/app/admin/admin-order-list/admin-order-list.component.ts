import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import OrderByDateResponse from '../../../backend/services/order/models/OrderByDateResponse';
import OrderByDateFilter from '../../../backend/services/order/models/OrderByDateFilter';
import OrderStatusResponse from '../../../backend/services/order/models/OrderStatusResponse';

@Component({
  selector: 'app-admin-order-list',
  templateUrl: './admin-order-list.component.html',
  standalone: false,
  styleUrls: ['./admin-order-list.component.css'],
})
export class AdminOrderListComponent implements OnInit {
  orders: OrderByDateResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  searchTerm: string = '';
  filterStatus: string = '';
  selectedOrder: OrderByDateResponse | null = null;
  processingOrderId: number | null = null;

  dateFrom: string = '';
  dateTo: string = '';

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
    this.setDefaultDates();
    this.loadOrders();
  }

  private setDefaultDates(): void {
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(monthAgo.getMonth() - 1);

    this.dateTo = this.formatDateInput(today);
    this.dateFrom = this.formatDateInput(monthAgo);
  }

  private formatDateInput(date: Date): string {
    const yyyy = date.getFullYear();
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  public loadOrders(): void {
    if (!this.dateFrom || !this.dateTo) {
      this.errorMessage = 'Las fechas son obligatorias';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const filters: OrderByDateFilter = {
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
    };

    if (this.filterStatus) {
      filters.status = Number(this.filterStatus);
    }

    this._orderService.getOrdersByDate(filters).subscribe({
      next: (data) => {
        this.orders = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar pedidos';
        this.loading = false;
      },
    });
  }

  public get filteredOrders(): OrderByDateResponse[] {
    if (!this.searchTerm.trim()) {
      return this.orders;
    }
    const term = this.searchTerm.toLowerCase();
    return this.orders.filter((order) => {
      const idMatch = order.orderId.toString().includes(term);
      const fullName =
        `${order.client.name} ${order.client.lastName}`.toLowerCase();
      return idMatch || fullName.includes(term);
    });
  }

  public openDetail(order: OrderByDateResponse): void {
    this.selectedOrder = order;
  }

  public closeDetail(): void {
    this.selectedOrder = null;
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

  public getStatusIcon(status: string): string {
    const map: Record<string, string> = {
      Pending: 'schedule',
      Prepared: 'restaurant_menu',
      Cancelled: 'cancel',
      OnTheWay: 'local_shipping',
      Delivered: 'check_circle',
      NotDelivered: 'block',
      Delayed: 'warning',
    };
    return map[status] || 'help';
  }

  public getTotalItems(order: OrderByDateResponse): number {
    return order.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  public canCancel(status: string): boolean {
    return status !== 'Delivered' && status !== 'Cancelled';
  }

  public cancelOrder(order: OrderByDateResponse): void {
    this.processingOrderId = order.orderId;

    this._orderService.cancelOrder(order.orderId).subscribe({
      next: (response) => {
        const idx = this.orders.findIndex((o) => o.orderId === order.orderId);
        if (idx !== -1) {
          this.orders[idx] = { ...this.orders[idx], status: response.status };
        }
        this.processingOrderId = null;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cancelar el pedido';
        this.processingOrderId = null;
      },
    });
  }
}
