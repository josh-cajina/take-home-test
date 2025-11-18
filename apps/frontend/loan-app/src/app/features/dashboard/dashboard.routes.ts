import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { roleGuard } from '../../core/guards/role-guard';
import { EditProfile } from './profile/edit-profile/edit-profile';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    component: Dashboard,
    children: [
      {
        path: 'requester',
        canActivate: [roleGuard],
        data: { role: 'Requester' },
        loadChildren: () => import('./requester/requester.routes').then(m => m.REQUESTER_ROUTES)
      },
      {
        path: 'analyst',
        canActivate: [roleGuard],
        data: { role: 'Analyst' },
        loadChildren: () => import('./analyst/analyst.routes').then(m => m.ANALYST_ROUTES)
      },
      {
        path: 'admin',
        canActivate: [roleGuard],
        data: { role: 'Admin' },
        loadChildren: () => import('./admin/admin.routes').then(m => m.ADMIN_ROUTES)
      },
      {
        path: 'profile',
        component: EditProfile
      },
      { path: '', children: [] }
    ]
  }
];
