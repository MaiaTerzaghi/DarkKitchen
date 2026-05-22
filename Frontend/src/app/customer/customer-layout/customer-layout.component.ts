import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-customer-layout',
  templateUrl: './customer-layout.component.html',
  standalone: false,
  styleUrls: ['./customer-layout.component.css'],
})
export class CustomerLayoutComponent {
  sidebarOpen: boolean = true;

  menuItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/customer' },
  ];

  constructor(
    private readonly _authService: AuthService,
    private readonly _router: Router
  ) {}

  public toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  public logout(): void {
    this._authService.logout();
    this._router.navigate(['/login']);
  }
}
