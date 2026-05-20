import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const isNotLoggedIn = localStorage.getItem('token') === null;

  if (isNotLoggedIn) {
    const router = inject(Router);
    return router.parseUrl('/login');
  }

  return true;
};
