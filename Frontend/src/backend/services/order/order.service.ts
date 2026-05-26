import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderApiRepositoryService } from '../../repositories/order-api-repository.service';
import CreateOrderRequest from './models/CreateOrderRequest';
import CreateOrderResponse from './models/CreateOrderResponse';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private readonly _repository: OrderApiRepositoryService) {}

  public createOrder(data: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this._repository.create(data);
  }
}
