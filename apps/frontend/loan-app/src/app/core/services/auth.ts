import { Injectable, inject, signal, computed } from '@angular/core';
import { ApiService } from './api';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { tap } from 'rxjs/operators';
import { AppUser } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = inject(ApiService);
  private router = inject(Router);
  private userSignal = signal<AppUser | null>(null);

  public user = computed(() => this.userSignal());

  constructor() { this.loadUser(); }

  private loadUser() {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        if (decoded.exp * 1000 > Date.now()) {
          this.userSignal.set({
            userId: decoded.sub,
            email: decoded.email,
            roles: Array.isArray(decoded.role) ? decoded.role : [decoded.role]
          });
        } else { this.logout(); }
      } catch { this.logout(); }
    }
  }

  login(creds: any) {
    return this.api.post<any>('users/login', creds).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        this.loadUser();
      })
    );
  }

  register(data: any) {
    return this.api.post<any>('users/register', data).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        this.loadUser();
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.userSignal.set(null);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated() { return !!this.userSignal(); }
  hasRole(role: string) { return this.userSignal()?.roles.includes(role) ?? false; }
  getToken() { return localStorage.getItem('token'); }
}
