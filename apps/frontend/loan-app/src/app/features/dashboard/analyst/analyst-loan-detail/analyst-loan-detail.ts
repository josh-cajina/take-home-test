import { Component, ChangeDetectionStrategy, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { LoanService } from '../../../../core/services/loan';
import { LoanDetails } from '../../../../core/models/loan';

@Component({
  selector: 'app-analyst-loan-detail',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './analyst-loan-detail.html',
  styleUrl: './analyst-loan-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnalystLoanDetail implements OnInit {
  private loanService = inject(LoanService);
  private fb = inject(FormBuilder);

  @Input() id!: string;
  loan = signal<LoanDetails | null>(null);
  error = signal<string | null>(null); // Added error signal

  paymentForm = this.fb.group({ amount: [0, [Validators.required, Validators.min(0.01)]] });
  statusForm = this.fb.group({ newStatusId: [3, Validators.required], comment: ['', Validators.required] });

  ngOnInit() { this.load(); }

  load() {
    this.error.set(null);
    this.loanService.getLoanById(this.id).subscribe({
      next: (d) => this.loan.set(d),
      error: (err) => {
        console.error(err);
        this.error.set('Failed to load loan details. Please try again.');
      }
    });
  }

  submitPay() {
    if (this.paymentForm.valid) {
      this.loanService.addPayment(this.id, this.paymentForm.value.amount!).subscribe(() => {
        this.paymentForm.reset();
        this.load();
      });
    }
  }

  submitStatus() {
    if (this.statusForm.valid) {
      const { newStatusId, comment } = this.statusForm.value;
      this.loanService.updateStatus(this.id, { loanId: this.id, newStatusId, comment }).subscribe(() => {
        this.statusForm.reset();
        this.load();
      });
    }
  }
}
