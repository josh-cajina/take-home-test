import { Routes } from '@angular/router';
import { UserManagement } from './user-management/user-management';

export const ADMIN_ROUTES: Routes = [
  { path: 'user-management', component: UserManagement },
  { path: '', redirectTo: 'user-management', pathMatch: 'full' }
];
