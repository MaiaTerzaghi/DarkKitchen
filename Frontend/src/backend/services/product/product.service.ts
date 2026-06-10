import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductApiRepositoryService } from '../../repositories/product-api-repository.service';
import ProductResponse from './models/ProductResponse';
import ProductManageFilter from './models/ProductManageFilter';
import CreateProductRequest from './models/CreateProductRequest';
import UpdateProductRequest from './models/UpdateProductRequest';
import PaginatedResponse from '../../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(
    private readonly _repository: ProductApiRepositoryService
  ) {}

  public getProducts(
    filters: ProductManageFilter = {}
  ): Observable<PaginatedResponse<ProductResponse>> {
    return this._repository.getProducts(filters);
  }

  public create(data: CreateProductRequest): Observable<ProductResponse> {
    return this._repository.create(data);
  }

  public update(
    id: number,
    data: UpdateProductRequest
  ): Observable<ProductResponse> {
    return this._repository.update(id, data);
  }
}