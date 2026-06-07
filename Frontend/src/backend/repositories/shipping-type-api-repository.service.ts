import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import ShippingTypeResponse from '../services/shipping-type/models/ShippingTypeResponse';

@Injectable({
  providedIn: 'root',
})
export class ShippingTypeApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'shipping-types', http);
  }

  public getAll(): Observable<ShippingTypeResponse[]> {
    return this.get<ShippingTypeResponse[]>();
  }
}
