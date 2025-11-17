import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet, Router, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Dashboard implements OnInit {
  auth = inject(AuthService);
  router = inject(Router);

  ngOnInit() {
    if (this.router.url === '/dashboard') {
      if (this.auth.hasRole('Requester')) this.router.navigate(['/dashboard/requester']);
      else if (this.auth.hasRole('Analyst')) this.router.navigate(['/dashboard/analyst']);
      else if (this.auth.hasRole('Admin')) this.router.navigate(['/dashboard/admin']);
    }
  }
}
