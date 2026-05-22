import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import PromotionResponse from '../services/promotion/models/PromotionResponse';
import PromotionFilter from '../services/promotion/models/PromotionFilter';
import CreatePromotionRequest from '../services/promotion/models/CreatePromotionRequest';
import UpdatePromotionRequest from '../services/promotion/models/UpdatePromotionRequest';

@Injectable({
  providedIn: 'root',
})
export class PromotionApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'promotions', http);
  }

  public getActivePromotions(
    filters: PromotionFilter
  ): Observable<PromotionResponse[]> {
    const params: string[] = [];

    if (filters.date) {
      params.push(`date=${filters.date}`);
    }
    if (filters.productLine) {
      params.push(`productLine=${filters.productLine}`);
    }
    if (filters.product) {
      params.push(`product=${filters.product}`);
    }

    const query = params.join('&');
    return this.get<PromotionResponse[]>('', query);
  }

  public create(
    data: CreatePromotionRequest
  ): Observable<PromotionResponse> {
    return this.post<PromotionResponse>(data);
  }

  public update(
    id: number,
    data: UpdatePromotionRequest
  ): Observable<PromotionResponse> {
    return this.putById<PromotionResponse>(id.toString(), data);
  }
}
