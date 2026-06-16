import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShippingTypeApiRepositoryService } from '../../repositories/shipping-type-api-repository.service';
import ShippingTypeResponse from './models/ShippingTypeResponse';

@Injectable({
  providedIn: 'root',
})
export class ShippingTypeService {
  constructor(private readonly _repository: ShippingTypeApiRepositoryService) {}

  public getAll(): Observable<ShippingTypeResponse[]> {
    return this._repository.getAll();
  }

  public create(data: { name: string; cost: number }): Observable<ShippingTypeResponse> {
    return this._repository.create(data);
  }

  public update(id: number, data: { name: string; cost: number }): Observable<ShippingTypeResponse> {
    return this._repository.update(id, data);
  }
}
