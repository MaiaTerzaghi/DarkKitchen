import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import OrderResponse from '../../../backend/services/order/models/OrderResponse';
import OrderFilter from '../../../backend/services/order/models/OrderFilter';
import OrderStatusResponse from '../../../backend/services/order/models/OrderStatusResponse';
import { OrderStatus } from '../../../backend/services/order/models/OrderStatus';

@Component({
  selector: 'app-admin-order-list',
  templateUrl: './admin-order-list.component.html',
  standalone: false,
  styleUrls: ['./admin-order-list.component.css'],
})
export class AdminOrderListComponent implements OnInit {
  orders: OrderResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  searchTerm: string = '';
  filterStatus: string = '';
  selectedOrder: OrderResponse | null = null;
  processingOrderId: number | null = null;

  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;

  dateFrom: string = '';
  dateTo: string = '';

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
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
    this.loading = true;
    this.errorMessage = '';

    const filters: OrderFilter = {};

    if (this.dateFrom) {
      filters.dateFrom = this.dateFrom;
    }
    if (this.dateTo) {
      filters.dateTo = this.dateTo;
    }

    filters.page = this.currentPage;
    filters.pageSize = this.pageSize;

    if (this.filterStatus) {
      filters.status = Number(this.filterStatus);
    }

    this._orderService.getOrders(filters).subscribe({
      next: (data) => {
        this.orders = data.items;
        this.totalCount = data.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar pedidos';
        this.loading = false;
      },
    });
  }

  public get filteredOrders(): OrderResponse[] {
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

  public openDetail(order: OrderResponse): void {
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

  public getTotalItems(order: OrderResponse): number {
    return order.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  public canCancel(status: string): boolean {
    return status !== 'Delivered' && status !== 'Cancelled';
  }

  public cancelOrder(order: OrderResponse): void {
    this.processingOrderId = order.orderId;

    this._orderService.changeStatus(order.orderId, OrderStatus.Cancelled).subscribe({
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

  public onPageChange(page: number): void {
    this.currentPage = page;
    this.loadOrders();
  }
}
