import { Injectable, inject } from '@angular/core';
import { ApiService } from './api';
import { UserProfile } from '../models/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  getProfile() {
    return this.api.get<UserProfile>('users/profile');
  }

  updateProfile(data: Partial<UserProfile>) {
    return this.api.put<void>('users/profile', data);
  }
}
