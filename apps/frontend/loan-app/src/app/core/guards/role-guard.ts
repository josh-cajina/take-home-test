import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return (auth.isAuthenticated() && auth.hasRole(route.data['role'])) || router.createUrlTree(['/dashboard']);
};
