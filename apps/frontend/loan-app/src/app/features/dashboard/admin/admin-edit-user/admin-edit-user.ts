import { Component, ChangeDetectionStrategy, inject, signal, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from '../../../../core/services/admin';
import { UserProfile } from '../../../../core/models/user';

@Component({
  selector: 'app-admin-edit-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-edit-user.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminEditUser implements OnInit {
  private adminService = inject(AdminService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  @Input() id!: string;

  protected isSubmitting = signal(false);
  protected successMsg = signal<string | null>(null);
  protected loading = signal(true);

  protected form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: [{value: '', disabled: true}],
    address: ['', Validators.required],
    dateOfBirth: ['', Validators.required]
  });

  ngOnInit() {
    this.loadUser();
  }

  private loadUser() {
    this.adminService.getUserById(this.id).subscribe({
      next: (data) => {
        const dob = data.dateOfBirth ? new Date(data.dateOfBirth).toISOString().split('T')[0] : '';
        this.form.patchValue({ ...data, dateOfBirth: dob });
        this.loading.set(false);
      },
      error: () => this.router.navigate(['/dashboard/admin/user-management'])
    });
  }

  protected onSubmit() {
    if (this.form.valid) {
      this.isSubmitting.set(true);
      this.successMsg.set(null);

      const profileData = this.form.getRawValue() as unknown as Partial<UserProfile>;

      this.adminService.updateUser(this.id, profileData).subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.successMsg.set('User updated successfully!');
          setTimeout(() => this.router.navigate(['/dashboard/admin/user-management']), 1500);
        },
        error: () => this.isSubmitting.set(false)
      });
    }
  }

  protected cancel() {
    this.router.navigate(['/dashboard/admin/user-management']);
  }
}
