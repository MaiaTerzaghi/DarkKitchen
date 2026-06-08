import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserApiRepositoryService } from '../../repositories/user-api-repository.service';
import RegisterRequest from './models/RegisterRequest';
import UserResponse from './models/UserResponse';
import CreateStaffUserRequest from './models/CreateStaffUserRequest';
import UpdateUserRequest from './models/UpdateUserRequest';
import PaginatedResponse from '../../models/PaginatedResponse';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private readonly _repository: UserApiRepositoryService) {}

  public register(data: RegisterRequest): Observable<number> {
    return this._repository.register(data);
  }

  public getUsers(name?: string, lastName?: string, page: number = 1, pageSize: number = 20): Observable<PaginatedResponse<UserResponse>> {
    return this._repository.getUsers(name, lastName, page, pageSize);
  }

  public createStaffUser(data: CreateStaffUserRequest): Observable<{ id: number }> {
    return this._repository.createStaffUser(data);
  }

  public updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this._repository.updateUser(id, data);
  }

  public deleteUser(id: number): Observable<void> {
    return this._repository.deleteUser(id);
  }
}
