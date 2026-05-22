import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserApiRepositoryService } from '../../repositories/user-api-repository.service';
import RegisterRequest from './models/RegisterRequest';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private readonly _repository: UserApiRepositoryService) {}

  public register(data: RegisterRequest): Observable<number> {
    return this._repository.register(data);
  }
}
