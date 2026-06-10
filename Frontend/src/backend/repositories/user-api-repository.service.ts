import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import CreateUserRequest from '../services/user/models/CreateUserRequest';
import UserResponse from '../services/user/models/UserResponse';
import UpdateUserRequest from '../services/user/models/UpdateUserRequest';
import PaginatedResponse from '../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class UserApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'users', http);
  }

  public createUser(data: CreateUserRequest): Observable<{ id: number }> {
    return this.post<{ id: number }>(data);
  }

  public getUsers(name?: string, lastName?: string, page: number = 1, pageSize: number = 20): Observable<PaginatedResponse<UserResponse>> {
    const params: string[] = [];
    if (name) params.push(`name=${name}`);
    if (lastName) params.push(`lastName=${lastName}`);
    params.push(`page=${page}`);
    params.push(`pageSize=${pageSize}`);
    const query = params.join('&');
    return this.get<PaginatedResponse<UserResponse>>('', query);
  }

  public updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this.putById<UserResponse>(id.toString(), data);
  }

  public deleteUser(id: number): Observable<void> {
    return this.delete<void>(`${id}`);
  }
}
