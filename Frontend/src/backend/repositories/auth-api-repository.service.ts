import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import LoginRequest from '../services/auth/models/LoginRequest';
import { LoginResponse } from '../services/auth/models/LoginResponse';

@Injectable({
  providedIn: 'root',
})
export class AuthApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'auth', http);
  }

  public login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.post(credentials, 'login'); // El primer parámetro es el body (email y password), el segundo es el la ruta extra que concatena el padre .
  }
}
