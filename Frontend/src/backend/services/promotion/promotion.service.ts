import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PromotionApiRepositoryService } from '../../repositories/promotion-api-repository.service';
import PromotionResponse from './models/PromotionResponse';
import PromotionFilter from './models/PromotionFilter';
import CreatePromotionRequest from './models/CreatePromotionRequest';
import UpdatePromotionRequest from './models/UpdatePromotionRequest';

@Injectable({
  providedIn: 'root',
})
export class PromotionService {
  constructor(
    private readonly _repository: PromotionApiRepositoryService
  ) {}

  public getActivePromotions(
    filters: PromotionFilter = {}
  ): Observable<PromotionResponse[]> {
    return this._repository.getActivePromotions(filters);
  }

  public create(
    data: CreatePromotionRequest
  ): Observable<PromotionResponse> {
    return this._repository.create(data);
  }

  public update(
    id: number,
    data: UpdatePromotionRequest
  ): Observable<PromotionResponse> {
    return this._repository.update(id, data);
  }
}
