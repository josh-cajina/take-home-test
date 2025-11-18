import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet, Router, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Dashboard implements OnInit { // Correct class name
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  ngOnInit() {

    if (this.router.url === '/dashboard') {
      if (this.auth.hasRole('Admin')) {
        this.router.navigate(['/dashboard/admin'], { replaceUrl: true });
      } else if (this.auth.hasRole('Analyst')) {
        this.router.navigate(['/dashboard/analyst'], { replaceUrl: true });
      } else if (this.auth.hasRole('Requester')) {
        this.router.navigate(['/dashboard/requester'], { replaceUrl: true });
      }
    }
  }

  logout() {
    this.auth.logout();
  }
}
