import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AuthApiRepositoryService } from '../../repositories/auth-api-repository.service';
import LoginRequest from './models/LoginRequest';
import LoginResponse from './models/LoginResponse';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private readonly _repository: AuthApiRepositoryService) {}

  public login(credentials: LoginRequest): Observable<LoginResponse> {
    return this._repository.login(credentials).pipe(
      tap((response) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('role', response.role);
      })
    );
  }

  public logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
  }

  public isLoggedIn(): boolean {
    return localStorage.getItem('token') !== null;
  }

  public getToken(): string | null {
    return localStorage.getItem('token');
  }

  public getRole(): string | null {
    return localStorage.getItem('role');
  }
}
