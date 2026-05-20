import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthApiRepositoryService } from '../../repositories/auth-api-repository.service';
import LoginRequest from './models/LoginRequest';
import { LoginResponse } from './models/LoginResponse';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private readonly _repository: AuthApiRepositoryService) {}

  public login(credentials: LoginRequest): Observable<LoginResponse> {
    return this._repository.login(credentials).pipe(
      tap((token) => {
        localStorage.setItem('token', token);
      })
    );
  }

  public logout(): void {
    localStorage.removeItem('token');
  }

  public isLoggedIn(): boolean {
    return localStorage.getItem('token') !== null;
  }

  public getToken(): string | null {
    return localStorage.getItem('token');
  }
}
