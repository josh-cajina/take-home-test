import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../../../../core/services/user';
import { UserProfile } from '../../../../core/models/user';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-profile.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditProfile implements OnInit {
  private userService = inject(UserService);
  private fb = inject(FormBuilder);

  protected isSubmitting = signal(false);
  protected successMsg = signal<string | null>(null);

  protected form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: [{value: '', disabled: true}],
    address: ['', Validators.required],
    dateOfBirth: ['', Validators.required]
  });

  ngOnInit() {
    this.userService.getProfile().subscribe(data => {
      const dob = data.dateOfBirth ? new Date(data.dateOfBirth).toISOString().split('T')[0] : '';
      this.form.patchValue({ ...data, dateOfBirth: dob });
    });
  }

  protected onSubmit() {
    if (this.form.valid) {
      this.isSubmitting.set(true);
      this.successMsg.set(null);

      const profileData = this.form.getRawValue() as unknown as Partial<UserProfile>;

      this.userService.updateProfile(profileData).subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.successMsg.set('Profile updated successfully!');
        },
        error: () => this.isSubmitting.set(false)
      });
    }
  }
}
