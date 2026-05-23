import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import ProductCatalogFilter from '../services/product/models/ProductCatalogFilter';
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

  public getManage(
    filters: ProductManageFilter
  ): Observable<ProductResponse[]> {
    const params: string[] = [];

    if (filters.name) {
      params.push(`Name=${filters.name}`);
    }
    if (filters.category) {
      params.push(`Category=${filters.category}`);
    }
    if (filters.commercialLine) {
      params.push(`CommercialLine=${filters.commercialLine}`);
    }
    if (filters.isActive !== undefined && filters.isActive !== null) {
      params.push(`IsActive=${filters.isActive}`);
    }
    if (filters.priceMin != null) {
      params.push(`PriceMin=${filters.priceMin}`);
    }
    if (filters.priceMax != null) {
      params.push(`PriceMax=${filters.priceMax}`);
    }

    const query = params.join('&');
    return this.get<ProductResponse[]>('manage', query);
  }

  public getCatalog(filters: ProductCatalogFilter): Observable<ProductResponse[]> {
    const params: string[] = [];

    if (filters.name) {
      params.push(`name=${filters.name}`);
    }
    if (filters.category) {
      params.push(`category=${filters.category}`);
    }
    if (filters.line) {
      params.push(`line=${filters.line}`);
    }

    const query = params.join('&');
    return this.get<ProductResponse[]>('', query);
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