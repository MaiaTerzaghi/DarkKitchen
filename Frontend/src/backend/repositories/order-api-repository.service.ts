import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import CreateOrderRequest from '../services/order/models/CreateOrderRequest';
import CreateOrderResponse from '../services/order/models/CreateOrderResponse';

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
}
