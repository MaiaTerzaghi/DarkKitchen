import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import RegisterRequest from '../services/user/models/RegisterRequest';
import UserResponse from '../services/user/models/UserResponse';
import CreateStaffUserRequest from '../services/user/models/CreateStaffUserRequest';
import UpdateUserRequest from '../services/user/models/UpdateUserRequest';

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

  public getUsers(name?: string, lastName?: string): Observable<UserResponse[]> {
    const params: string[] = [];
    if (name) params.push(`name=${name}`);
    if (lastName) params.push(`lastName=${lastName}`);
    const query = params.join('&');
    return this.get<UserResponse[]>('', query);
  }

  public createStaffUser(data: CreateStaffUserRequest): Observable<{ id: number }> {
    return this.post<{ id: number }>(data, 'staff');
  }

  public updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this.putById<UserResponse>(id.toString(), data);
  }

  public deleteUser(id: number): Observable<void> {
    return this.delete<void>(`${id}`);
  }
}
