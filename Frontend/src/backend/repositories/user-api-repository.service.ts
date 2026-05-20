import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import RegisterRequest from '../services/user/models/RegisterRequest';

@Injectable({
  providedIn: 'root',
})
export class UserApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'users', http);
  }

  public register(data: RegisterRequest): Observable<number> {
    return this.post<number>(data);
  }
}
