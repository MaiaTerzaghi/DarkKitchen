import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderService } from '../../../backend/services/order/order.service';
import OrderResponse from '../../../backend/services/order/models/OrderResponse';
import OrderStatusResponse from '../../../backend/services/order/models/OrderStatusResponse';

interface OrderAction {
  label: string;
  icon: string;
  cssClass: string;
  canApply: (status: string) => boolean;
  apply: (id: number) => Observable<OrderStatusResponse>;
}

interface ToastMessage {
  message: string;
  type: 'success' | 'error';
}

@Component({
  selector: 'app-order-management',
  templateUrl: './order-management.component.html',
  standalone: false,
  styleUrls: ['./order-management.component.css'],
})
export class OrderManagementComponent implements OnInit {
  orders: OrderResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  searchTerm: string = '';
  toast: ToastMessage | null = null;
  processingOrderId: number | null = null;

  orderActions: OrderAction[] = [
    {
      label: 'Marcar como Preparado',
      icon: 'check_circle',
      cssClass: 'btn-prepare',
      canApply: (status) => status === 'Pending',
      apply: (id) => this._orderService.markAsPrepared(id),
    },
    {
      label: 'Marcar como En camino',
      icon: 'local_shipping',
      cssClass: 'btn-on-the-way',
      canApply: (status) => status === 'Prepared',
      apply: (id) => this._orderService.markAsOnTheWay(id),
    },
    {
      label: 'Marcar como Entregado',
      icon: 'task_alt',
      cssClass: 'btn-deliver',
      canApply: (status) => status === 'OnTheWay',
      apply: (id) => this._orderService.deliverOrder(id),
    },
    {
      label: 'Marcar como No entregado',
      icon: 'block',
      cssClass: 'btn-not-delivered',
      canApply: (status) => status === 'OnTheWay',
      apply: (id) => this._orderService.markAsNotDelivered(id),
    },
  ];

  constructor(private readonly _orderService: OrderService) {}

  public ngOnInit(): void {
    this.loadOrders();
  }

  public loadOrders(): void {
    this.loading = true;
    this.errorMessage = '';

    this._orderService.getDispatcherOrders().subscribe({
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

  public get filteredOrders(): OrderResponse[] {
    if (!this.searchTerm.trim()) {
      return this.orders;
    }
    const term = this.searchTerm.toLowerCase();
    return this.orders.filter((order) => {
      const idMatch = order.orderId.toString().includes(term);
      const fullName = `${order.client.name} ${order.client.lastName}`.toLowerCase();
      return idMatch || fullName.includes(term);
    });
  }

  public availableActions(status: string): OrderAction[] {
    return this.orderActions.filter((action) => action.canApply(status));
  }

  public executeAction(action: OrderAction, order: OrderResponse): void {
    this.processingOrderId = order.orderId;

    action.apply(order.orderId).subscribe({
      next: (response) => {
        const idx = this.orders.findIndex((o) => o.orderId === order.orderId);
        if (idx !== -1) {
          this.orders[idx] = { ...this.orders[idx], status: response.status };
        }
        this.showToast(
          `Pedido #${response.orderId} marcado como ${this.getStatusLabel(
            response.status
          )} el ${this.formatDate(response.updatedAt)}`,
          'success'
        );
        this.processingOrderId = null;
      },
      error: (err) => {
        this.showToast(err || 'No se pudo realizar la operación.', 'error');
        this.processingOrderId = null;
      },
    });
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

  private formatDate(isoDate: string): string {
    const date = new Date(isoDate);
    const dd = String(date.getDate()).padStart(2, '0');
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const yyyy = date.getFullYear();
    const hh = String(date.getHours()).padStart(2, '0');
    const mi = String(date.getMinutes()).padStart(2, '0');
    return `${dd}/${mm}/${yyyy} ${hh}:${mi}`;
  }

  private showToast(message: string, type: 'success' | 'error'): void {
    this.toast = { message, type };
    setTimeout(() => {
      this.toast = null;
    }, 4000);
  }
}