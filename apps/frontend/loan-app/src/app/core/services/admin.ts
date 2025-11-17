import { Injectable, inject } from '@angular/core';
import { ApiService } from './api';
import { AdminUserView } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = inject(ApiService);
  getAllUsers() { return this.api.get<AdminUserView[]>('users/all'); }
}
