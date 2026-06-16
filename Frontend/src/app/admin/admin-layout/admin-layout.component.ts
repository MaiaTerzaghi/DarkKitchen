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
    { icon: 'receipt_long', label: 'Pedidos', route: '/admin/orders' },
    { icon: 'local_offer', label: 'Promociones', route: '/admin/promotions' },
    { icon: 'inventory_2', label: 'Productos', route: '/admin/products' },
    { icon: 'upload_file', label: 'Importar Productos', route: '/admin/import' },
    { icon: 'local_shipping', label: 'Tipos de envío', route: '/admin/shipping-types' },
    { icon: 'assessment', label: 'Reporte de ventas', route: '/admin/report' },
    { icon: 'history', label: 'Auditoría', route: '/admin/audit' },
    { icon: 'people', label: 'Usuarios', route: '/admin/users' },
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
