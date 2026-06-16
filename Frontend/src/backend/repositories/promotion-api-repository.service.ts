import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import PromotionResponse from '../services/promotion/models/PromotionResponse';
import PromotionFilter from '../services/promotion/models/PromotionFilter';
import CreatePromotionRequest from '../services/promotion/models/CreatePromotionRequest';
import UpdatePromotionRequest from '../services/promotion/models/UpdatePromotionRequest';
import PaginatedResponse from '../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class PromotionApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'promotions', http);
  }

  public getActivePromotions(
    filters: PromotionFilter
  ): Observable<PaginatedResponse<PromotionResponse>> {
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

    params.push(`Page=${filters.page ?? 1}`);
    params.push(`PageSize=${filters.pageSize ?? 20}`);

    const query = params.join('&');
    return this.get<PaginatedResponse<PromotionResponse>>('', query);
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

  public addProduct(
    promotionId: number,
    productId: number
  ): Observable<void> {
    return this.post<void>(null, `${promotionId}/products?productId=${productId}`);
  }

  public removeProduct(
    promotionId: number,
    productId: number
  ): Observable<void> {
    return this.delete<void>(`${promotionId}/products`, `productId=${productId}`);
  }
}
