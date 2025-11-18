import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LoanService } from '../../../../core/services/loan';

@Component({
  selector: 'app-new-loan',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './new-loan.html',
  styleUrl: './new-loan.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NewLoan {
  private fb = inject(FormBuilder);
  private loanService = inject(LoanService);
  private router = inject(Router);

  // Signals for UI state
  protected isSubmitting = signal(false);

  // --- THESE ARE THE SIGNALS YOUR HTML IS LOOKING FOR ---
  protected errorMessage = signal<string | null>(null);
  protected successMessage = signal<string | null>(null);

  protected form = this.fb.group({
    requestedAmount: [null, [Validators.required, Validators.min(100)]],
    termInMonths: [12, [Validators.required, Validators.min(1), Validators.max(120)]],
    purpose: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]]
  });

  protected isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  protected onCancel() {
    this.router.navigate(['/dashboard/requester']);
  }

  protected onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.loanService.createLoan(this.form.value).subscribe({
      next: (loanId) => {
        this.successMessage.set(`Loan request submitted successfully!`);
        this.form.reset();
        setTimeout(() => {
          this.router.navigate(['/dashboard/requester/my-loans']);
        }, 1500);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        // Set the specific signal here
        this.errorMessage.set('An error occurred while submitting your request. Please try again.');
      }
    });
  }
}
