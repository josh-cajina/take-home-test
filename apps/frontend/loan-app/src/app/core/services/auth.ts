import { Injectable, inject, signal, computed } from '@angular/core';
import { ApiService } from './api';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { tap } from 'rxjs/operators';
import { AppUser } from '../models/user';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = inject(ApiService);
  private router = inject(Router);
  private readonly tokenKey = 'loan_app_token';

  private userSignal = signal<AppUser | null>(null);

  public user = computed(() => this.userSignal());

  constructor() {
    this.loadUserFromToken();
  }

  private loadUserFromToken() {
    const token = localStorage.getItem(this.tokenKey);
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        // Check expiration (exp is in seconds, Date.now is ms)
        if (decoded.exp * 1000 > Date.now()) {
          this.userSignal.set(this.parseUser(decoded));
        } else {
          this.logout();
        }
      } catch {
        this.logout();
      }
    }
  }

  private parseUser(decoded: any): AppUser {

    const dotnetRoleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    // 2. Look for the role in the token using either key
    const rawRole = decoded[dotnetRoleKey] || decoded.role || [];

    // 3. Ensure it's always an array (even if it's a single string like "Analyst")
    const roles = Array.isArray(rawRole) ? rawRole : [rawRole];

    return {
      userId: decoded.sub,
      email: decoded.email,
      roles: roles
    };
  }

  login(creds: any): Observable<any> {
    return this.api.post<any>('users/login', creds).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
        this.userSignal.set(this.parseUser(jwtDecode(res.token)));
      })
    );
  }

  register(data: any): Observable<any> {
    return this.api.post<any>('users/register', data).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
        this.userSignal.set(this.parseUser(jwtDecode(res.token)));
      })
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.userSignal.set(null);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated() {
    return !!this.userSignal();
  }

  hasRole(role: string) {
    const currentUser = this.userSignal();
    return currentUser?.roles.includes(role) ?? false;
  }

  getToken() {
    return localStorage.getItem(this.tokenKey);
  }
}
