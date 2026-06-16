import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-dispatcher-layout',
  templateUrl: './dispatcher-layout.component.html',
  standalone: false,
  styleUrls: ['./dispatcher-layout.component.css'],
})
export class DispatcherLayoutComponent {
  sidebarOpen: boolean = true;

  menuItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/dispatcher' },
    { icon: 'assignment', label: 'Gestión de Pedidos', route: '/dispatcher/management' },
    { icon: 'receipt_long', label: 'Pedidos', route: '/dispatcher/orders' },
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