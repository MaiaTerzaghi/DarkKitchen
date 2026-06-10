import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import CreateUserRequest from '../services/user/models/CreateUserRequest';
import UserResponse from '../services/user/models/UserResponse';
import UpdateUserRequest from '../services/user/models/UpdateUserRequest';

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

  public getUsers(name?: string, lastName?: string): Observable<UserResponse[]> {
    const params: string[] = [];
    if (name) params.push(`name=${name}`);
    if (lastName) params.push(`lastName=${lastName}`);
    const query = params.join('&');
    return this.get<UserResponse[]>('', query);
  }

  public updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this.putById<UserResponse>(id.toString(), data);
  }

  public deleteUser(id: number): Observable<void> {
    return this.delete<void>(`${id}`);
  }
}
