import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserApiRepositoryService } from '../../repositories/user-api-repository.service';
import CreateUserRequest from './models/CreateUserRequest';
import UserResponse from './models/UserResponse';
import UpdateUserRequest from './models/UpdateUserRequest';
import PaginatedResponse from '../../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private readonly _repository: UserApiRepositoryService) {}

  public createUser(data: CreateUserRequest): Observable<{ id: number }> {
    return this._repository.createUser(data);
  }

  public getUsers(name?: string, lastName?: string, page: number = 1, pageSize: number = 20): Observable<PaginatedResponse<UserResponse>> {
    return this._repository.getUsers(name, lastName, page, pageSize);
  }

  public updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this._repository.updateUser(id, data);
  }

  public deleteUser(id: number): Observable<void> {
    return this._repository.deleteUser(id);
  }
}
