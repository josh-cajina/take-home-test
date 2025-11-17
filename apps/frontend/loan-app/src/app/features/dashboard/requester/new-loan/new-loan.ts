import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoanService } from '../../../../core/services/loan';

@Component({
  selector: 'app-new-loan',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './new-loan.html',
  styleUrl: './new-loan.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NewLoan {
  private fb = inject(FormBuilder);
  private loanService = inject(LoanService);
  private router = inject(Router);

  form = this.fb.group({
    requestedAmount: [null, [Validators.required, Validators.min(100)]],
    termInMonths: [12, Validators.required],
    purpose: ['', Validators.required]
  });

  onSubmit() {
    if (this.form.valid) {
      this.loanService.createLoan(this.form.value).subscribe(() => {
        this.router.navigate(['/dashboard/requester/my-loans']);
      });
    }
  }
}
