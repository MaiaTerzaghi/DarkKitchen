import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  standalone: false,
  styleUrls: ['./admin-layout.component.css'],
})
export class AdminLayoutComponent {
  sidebarOpen: boolean = true;

  menuItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/admin' },
    { icon: 'local_offer', label: 'Promociones', route: '/admin/promotions' },
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
