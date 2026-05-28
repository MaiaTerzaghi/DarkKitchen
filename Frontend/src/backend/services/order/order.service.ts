import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderApiRepositoryService } from '../../repositories/order-api-repository.service';
import CreateOrderRequest from './models/CreateOrderRequest';
import CreateOrderResponse from './models/CreateOrderResponse';
import OrderResponse from './models/OrderResponse';
import OrderFilter from './models/OrderFilter';
import OrderByDateResponse from './models/OrderByDateResponse';
import OrderByDateFilter from './models/OrderByDateFilter';
import OrderStatusResponse from './models/OrderStatusResponse';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private readonly _repository: OrderApiRepositoryService) {}

  public createOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this._repository.create(data);
  }

  public getClientOrders(filters: OrderFilter = {}): Observable<OrderResponse[]> {
    return this._repository.getClientOrders(filters);
  }

  public getOrdersByDate(
    filters: OrderByDateFilter
  ): Observable<OrderByDateResponse[]> {
    return this._repository.getOrdersByDate(filters);
  }

  public getDispatcherOrders(): Observable<OrderByDateResponse[]> {
    return this._repository.getDispatcherOrders();
  }

  public markAsPrepared(id: number): Observable<OrderStatusResponse> {
    return this._repository.markAsPrepared(id);
  }

  public deliverOrder(id: number): Observable<OrderStatusResponse> {
    return this._repository.deliverOrder(id);
  }
}