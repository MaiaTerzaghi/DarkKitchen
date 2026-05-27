import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import OrderResponse from '../services/order/models/OrderResponse';
import OrderFilter from '../services/order/models/OrderFilter';
import OrderByDateResponse from '../services/order/models/OrderByDateResponse';
import OrderByDateFilter from '../services/order/models/OrderByDateFilter';
import OrderStatusResponse from '../services/order/models/OrderStatusResponse';

@Injectable({
  providedIn: 'root',
})
export class OrderApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'orders', http);
  }

  public getClientOrders(filters: OrderFilter): Observable<OrderResponse[]> {
    const params: string[] = [];

    if (filters.dateFrom) {
      params.push(`DateFrom=${filters.dateFrom}`);
    }
    if (filters.dateTo) {
      params.push(`DateTo=${filters.dateTo}`);
    }
    if (filters.status) {
      params.push(`Status=${filters.status}`);
    }

    const query = params.join('&');
    return this.get<OrderResponse[]>('', query);
  }

  public getOrdersByDate(
    filters: OrderByDateFilter
  ): Observable<OrderByDateResponse[]> {
    const params: string[] = [
      `DateFrom=${filters.dateFrom}`,
      `DateTo=${filters.dateTo}`,
    ];

    if (filters.street) {
      params.push(`Street=${filters.street}`);
    }
    if (filters.status !== undefined && filters.status !== null) {
      params.push(`Status=${filters.status}`);
    }

    const query = params.join('&');
    return this.get<OrderByDateResponse[]>('by-date', query);
  }

  public getDispatcherOrders(): Observable<OrderByDateResponse[]> {
    return this.get<OrderByDateResponse[]>('dispatcher');
  }

  public markAsPrepared(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'prepared');
  }

  public deliverOrder(id: number): Observable<OrderStatusResponse> {
    return this.patch<OrderStatusResponse>(id, 'deliver');
  }
}