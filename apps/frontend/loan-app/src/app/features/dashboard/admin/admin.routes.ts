import { Routes } from '@angular/router';
import { UserManagement} from './user-management/user-management';
import { AdminHome } from './admin-home/admin-home';
import { AdminEditUser } from './admin-edit-user/admin-edit-user';

export const ADMIN_ROUTES: Routes = [
  { path: '', component: AdminHome },
  { path: 'user-management', component: UserManagement },
  { path: 'users/:id/edit', component: AdminEditUser }
];
