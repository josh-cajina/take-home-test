import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../../core/services/admin';
import { AdminUserView } from '../../../../core/models/user';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserManagement implements OnInit {
  private adminService = inject(AdminService);
  users = signal<AdminUserView[]>([]);

  ngOnInit() {
    this.adminService.getAllUsers().subscribe(data => this.users.set(data));
  }
}
