import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES),
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard/dashboard').then(c => c.Dashboard),
    canActivate: [authGuard],
    children: [
      {
        path: 'requester',
        canActivate: [roleGuard],
        data: { role: 'Requester' },
        loadChildren: () => import('./features/dashboard/requester/requester.routes').then(m => m.REQUESTER_ROUTES)
      },
      {
        path: 'analyst',
        canActivate: [roleGuard],
        data: { role: 'Analyst' },
        loadChildren: () => import('./features/dashboard/analyst/analyst.routes').then(m => m.ANALYST_ROUTES)
      },
      {
        path: 'admin',
        canActivate: [roleGuard],
        data: { role: 'Admin' },
        loadChildren: () => import('./features/dashboard/admin/admin.routes').then(m => m.ADMIN_ROUTES)
      },
      { path: '', redirectTo: 'requester', pathMatch: 'full' }
    ]
  },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
