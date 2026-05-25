import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderApiRepositoryService } from '../../repositories/order-api-repository.service';
import OrderResponse from './models/OrderResponse';
import OrderFilter from './models/OrderFilter';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private readonly _repository: OrderApiRepositoryService) {}

  public getClientOrders(filters: OrderFilter = {}): Observable<OrderResponse[]> {
    return this._repository.getClientOrders(filters);
  }
}