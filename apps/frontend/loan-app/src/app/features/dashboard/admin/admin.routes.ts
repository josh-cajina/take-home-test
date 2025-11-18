import { Routes } from '@angular/router';
import { UserManagement } from './user-management/user-management';
import { AdminEditUser } from './admin-edit-user/admin-edit-user';

export const ADMIN_ROUTES: Routes = [
  { path: 'user-management', component: UserManagement },
  { path: 'users/:id/edit', component: AdminEditUser },
  { path: '', redirectTo: 'user-management', pathMatch: 'full' }
];
