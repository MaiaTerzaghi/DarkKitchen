import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderApiRepositoryService } from '../../repositories/order-api-repository.service';
import CreateOrderRequest from './models/CreateOrderRequest';
import CreateOrderResponse from './models/CreateOrderResponse';
import OrderResponse from './models/OrderResponse';
import OrderFilter from './models/OrderFilter';
import OrderStatusResponse from './models/OrderStatusResponse';
import OrderPreviewResponse from './models/OrderPreviewResponse';
import TopProductResponse from './models/TopProductResponse';
import SalesReportResponse from './models/SalesReportResponse';
import PaginatedResponse from '../../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private readonly _repository: OrderApiRepositoryService) {}

  public createOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this._repository.create(data);
  }

  public previewOrder(items: { productId: number; quantity: number }[], shippingType: string): Observable<OrderPreviewResponse> {
    return this._repository.previewOrder({ items, shippingType });
  }

  public getOrders(filters: OrderFilter = {}): Observable<PaginatedResponse<OrderResponse>> {
    return this._repository.getOrders(filters);
  }

  public getDispatcherOrders(): Observable<OrderResponse[]> {
    return this._repository.getDispatcherOrders();
  }

  public markAsPrepared(id: number): Observable<OrderStatusResponse> {
    return this._repository.markAsPrepared(id);
  }

  public deliverOrder(id: number): Observable<OrderStatusResponse> {
    return this._repository.deliverOrder(id);
  }

  public markAsNotDelivered(id: number): Observable<OrderStatusResponse> {
    return this._repository.markAsNotDelivered(id);
  }

  public getTopProducts(dateFrom: string, dateTo: string): Observable<TopProductResponse[]> {
    return this._repository.getTopProducts(dateFrom, dateTo);
  }

  public getSalesReport(page: number, pageSize: number): Observable<SalesReportResponse> {
    return this._repository.getSalesReport(page, pageSize);
  }

  public markAsOnTheWay(id: number): Observable<OrderStatusResponse> {
    return this._repository.markAsOnTheWay(id);
  }

  public cancelOrder(id: number): Observable<OrderStatusResponse> {
    return this._repository.cancelOrder(id);
  }
}