import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import TopProductResponse from '../../../backend/services/order/models/TopProductResponse';
import OrderResponse from '../../../backend/services/order/models/OrderResponse';
import OrderFilter from '../../../backend/services/order/models/OrderFilter';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  standalone: false,
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit {
  topProducts: TopProductResponse[] = [];
  loadingTop: boolean = false;
  errorTop: string = '';

  orders: OrderResponse[] = [];
  loadingOrders: boolean = false;

  dateFrom: string = '';
  dateTo: string = '';

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
    this.setDefaultDates();
    this.loadAll();
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

  public loadAll(): void {
    this.loadTopProducts();
    this.loadOrders();
  }

  public loadTopProducts(): void {
    if (!this.dateFrom || !this.dateTo) {
      this.errorTop = 'Las fechas son obligatorias';
      return;
    }

    this.loadingTop = true;
    this.errorTop = '';

    this._orderService.getTopProducts(this.dateFrom, this.dateTo).subscribe({
      next: (data) => {
        this.topProducts = data;
        this.loadingTop = false;
      },
      error: (err) => {
        this.errorTop = err || 'Error al cargar productos más vendidos';
        this.loadingTop = false;
      },
    });
  }

  private loadOrders(): void {
    if (!this.dateFrom || !this.dateTo) return;

    this.loadingOrders = true;

    const filters: OrderFilter = {
      dateFrom: this.dateFrom,
      dateTo: this.dateTo,
    };

    this._orderService.getOrders(filters).subscribe({
      next: (data) => {
        this.orders = data.items;
        this.loadingOrders = false;
      },
      error: () => {
        this.loadingOrders = false;
      },
    });
  }

  public get totalOrders(): number {
    return this.orders.length;
  }

  public get pendingOrders(): number {
    return this.orders.filter((o) => o.status === 'Pending').length;
  }

  public get onTheWayOrders(): number {
    return this.orders.filter((o) => o.status === 'OnTheWay').length;
  }

  public get deliveredOrders(): number {
    return this.orders.filter((o) => o.status === 'Delivered').length;
  }

  public get recentOrders(): OrderResponse[] {
    return [...this.orders]
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 5);
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

  public getImageList(images: string): string[] {
    if (!images) return [];
    return images.split(',').map((img) => img.trim()).filter((img) => img.length > 0);
  }
}
