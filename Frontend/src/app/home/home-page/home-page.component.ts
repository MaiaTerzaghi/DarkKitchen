import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  standalone: false,
  styles: [],
})
export class HomePageComponent {
  constructor(
    private readonly _authService: AuthService,
    private readonly _router: Router
  ) {}

  public logout(): void {
    this._authService.logout();
    this._router.navigate(['/login']);
  }
}
