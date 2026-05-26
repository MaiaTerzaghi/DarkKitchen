import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import ProductResponse from '../services/product/models/ProductResponse';

@Injectable({
  providedIn: 'root',
})
export class ProductApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'products', http);
  }

  public getAll(query: string = ''): Observable<ProductResponse[]> {
    return this.get<ProductResponse[]>('', query);
  }
}
