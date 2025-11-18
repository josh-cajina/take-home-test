import { Injectable, inject } from '@angular/core';
import { ApiService } from './api';
import { AdminUserView, UserProfile } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = inject(ApiService);

  getAllUsers() {
    return this.api.get<AdminUserView[]>('users/all');
  }

  getUserById(id: string) {
    return this.api.get<UserProfile>(`users/${id}`);
  }

  updateUser(id: string, data: Partial<UserProfile>) {
    return this.api.put<void>(`users/${id}`, data);
  }
}
