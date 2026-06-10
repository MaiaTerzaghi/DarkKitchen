import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import CreateOrderRequest from '../services/order/models/CreateOrderRequest';
import CreateOrderResponse from '../services/order/models/CreateOrderResponse';
import environments from '../../environments/environment';
import OrderResponse from '../services/order/models/OrderResponse';
import OrderFilter from '../services/order/models/OrderFilter';
import OrderStatusResponse from '../services/order/models/OrderStatusResponse';
import OrderPreviewResponse from '../services/order/models/OrderPreviewResponse';
import TopProductResponse from '../services/order/models/TopProductResponse';
import SalesReportResponse from '../services/order/models/SalesReportResponse';
import PaginatedResponse from '../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class OrderApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'orders', http);
  }

  public create(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this.post<CreateOrderResponse>(data);
  }

  public previewOrder(data: { items: { productId: number; quantity: number }[]; shippingType: string }): Observable<OrderPreviewResponse> {
    return this.post<OrderPreviewResponse>(data, 'preview');
  }

  public getOrders(filters: OrderFilter = {}): Observable<PaginatedResponse<OrderResponse>> {
    const params: string[] = [];

    if (filters.dateFrom) params.push(`DateFrom=${filters.dateFrom}`);
    if (filters.dateTo) params.push(`DateTo=${filters.dateTo}`);
    if (filters.street) params.push(`Street=${filters.street}`);
    if (filters.status !== undefined && filters.status !== null) params.push(`Status=${filters.status}`);

    params.push(`Page=${filters.page ?? 1}`);
    params.push(`PageSize=${filters.pageSize ?? 20}`);

    const query = params.join('&');
    return this.get<PaginatedResponse<OrderResponse>>('', query);
  }

  public getDispatcherOrders(): Observable<OrderResponse[]> {
    return this.get<OrderResponse[]>('dispatcher');
  }

  public markAsPrepared(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'prepared');
  }

  public deliverOrder(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'deliver');
  }

  public markAsNotDelivered(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'not-delivered');
  }

  public getTopProducts(dateFrom: string, dateTo: string): Observable<TopProductResponse[]> {
    const query = `DateFrom=${dateFrom}&DateTo=${dateTo}`;
    return this.get<TopProductResponse[]>('top-products', query);
  }

  public getSalesReport(page: number, pageSize: number): Observable<SalesReportResponse> {
    const query = `page=${page}&pageSize=${pageSize}`;
    return this.get<SalesReportResponse>('report', query);
  }

  public markAsOnTheWay(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'on-the-way');
  }

  public cancelOrder(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'cancel');
  }
}
