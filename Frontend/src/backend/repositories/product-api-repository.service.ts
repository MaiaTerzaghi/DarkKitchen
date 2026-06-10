import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import ProductResponse from '../services/product/models/ProductResponse';
import ProductManageFilter from '../services/product/models/ProductManageFilter';
import CreateProductRequest from '../services/product/models/CreateProductRequest';
import UpdateProductRequest from '../services/product/models/UpdateProductRequest';

@Injectable({
  providedIn: 'root',
})
export class ProductApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'products', http);
  }

  public getProducts(filters: ProductManageFilter = {}): Observable<ProductResponse[]> {
    const params: string[] = [];

    if (filters.name) params.push(`Name=${encodeURIComponent(filters.name)}`);
    if (filters.category) params.push(`Category=${encodeURIComponent(filters.category)}`);
    if (filters.commercialLine) params.push(`CommercialLine=${encodeURIComponent(filters.commercialLine)}`);
    if (filters.isActive !== undefined && filters.isActive !== null) params.push(`IsActive=${filters.isActive}`);
    if (filters.priceMin != null) params.push(`PriceMin=${filters.priceMin}`);
    if (filters.priceMax != null) params.push(`PriceMax=${filters.priceMax}`);
    if (filters.page) params.push(`Page=${filters.page}`);
    if (filters.pageSize) params.push(`PageSize=${filters.pageSize}`);

    return this.get<ProductResponse[]>('', params.join('&'));
  }

  public create(data: CreateProductRequest): Observable<ProductResponse> {
    return this.post<ProductResponse>(data);
  }

  public update(
    id: number,
    data: UpdateProductRequest
  ): Observable<ProductResponse> {
    return this.putById<ProductResponse>(id.toString(), data);
  }
}