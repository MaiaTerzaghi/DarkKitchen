import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const noAuthGuard: CanActivateFn = (route, state) => {
  const isLoggedIn = localStorage.getItem('token') !== null;

  if (isLoggedIn) {
    const router = inject(Router);
    return router.parseUrl('/home');
  }

  return true;
};
