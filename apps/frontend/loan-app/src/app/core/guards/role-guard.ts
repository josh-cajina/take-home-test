import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const expectedRole = route.data['role'];

  if (!auth.isAuthenticated()) {
    return router.createUrlTree(['/auth/login']);
  }

  if (auth.hasRole(expectedRole)) {
    return true;
  }

  if (auth.hasRole('Admin')) {
    return router.createUrlTree(['/dashboard/admin']);
  } else if (auth.hasRole('Analyst')) {
    return router.createUrlTree(['/dashboard/analyst']);
  } else {
    return router.createUrlTree(['/dashboard/requester']);
  }
};
