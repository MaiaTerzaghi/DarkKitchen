import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductApiRepositoryService } from '../../repositories/product-api-repository.service';
import ProductResponse from './models/ProductResponse';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private readonly _repository: ProductApiRepositoryService) {}

  public getAll(
    name?: string,
    category?: string,
    line?: string,
    page: number = 1,
    pageSize: number = 20
  ): Observable<ProductResponse[]> {
    const params: string[] = [];
    if (name) params.push(`name=${encodeURIComponent(name)}`);
    if (category) params.push(`category=${encodeURIComponent(category)}`);
    if (line) params.push(`line=${encodeURIComponent(line)}`);
    params.push(`page=${page}`);
    params.push(`pageSize=${pageSize}`);
    return this._repository.getAll(params.join('&'));
  }
}
