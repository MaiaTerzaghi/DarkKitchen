import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderApiRepositoryService } from '../../repositories/order-api-repository.service';
import CreateOrderRequest from './models/CreateOrderRequest';
import CreateOrderResponse from './models/CreateOrderResponse';
import OrderResponse from './models/OrderResponse';
import OrderFilter from './models/OrderFilter';
import OrderStatusResponse from './models/OrderStatusResponse';
import { OrderStatus } from './models/OrderStatus';
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

  public getDispatcherOrders(page: number = 1, pageSize: number = 20): Observable<PaginatedResponse<OrderResponse>> {
    return this._repository.getDispatcherOrders(page, pageSize);
  }

  public changeStatus(id: number, status: OrderStatus): Observable<OrderStatusResponse> {
    return this._repository.changeStatus(id, status);
  }

  public getTopProducts(dateFrom: string, dateTo: string): Observable<TopProductResponse[]> {
    return this._repository.getTopProducts(dateFrom, dateTo);
  }

  public getSalesReport(page: number, pageSize: number): Observable<SalesReportResponse> {
    return this._repository.getSalesReport(page, pageSize);
  }
}