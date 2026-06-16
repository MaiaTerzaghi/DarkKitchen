import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const noAuthGuard: CanActivateFn = (route, state) => {
  const isLoggedIn = localStorage.getItem('token') !== null;

  if (isLoggedIn) {
    const router = inject(Router);
    const role = localStorage.getItem('role');

    switch (role) {
      case 'Administrative':
        return router.parseUrl('/admin');
      case 'Dispatcher':
        return router.parseUrl('/dispatcher');
      case 'Client':
        return router.parseUrl('/customer');
      default:
        return router.parseUrl('/login');
    }
  }

  return true;
};
