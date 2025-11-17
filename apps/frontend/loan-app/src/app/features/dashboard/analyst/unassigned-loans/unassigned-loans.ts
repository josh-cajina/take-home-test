import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { LoanService } from '../../../../core/services/loan';
import { LoanBrief } from '../../../../core/models/loan';

@Component({
  selector: 'app-unassigned-loans',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './unassigned-loans.html',
  styleUrl: './unassigned-loans.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UnassignedLoans implements OnInit {
  private loanService = inject(LoanService);
  private router = inject(Router);
  loans = signal<LoanBrief[]>([]);

  ngOnInit() {
    this.loanService.getAllLoans().pipe(
      map(loans => loans.filter(l => l.analystFullName === 'Unassigned'))
    ).subscribe(data => this.loans.set(data));
  }

  claim(id: string) {
    this.loanService.updateStatus(id, { loanId: id, newStatusId: 2, comment: 'Claimed' }).subscribe(() => {
      this.router.navigate(['/dashboard/analyst/loan', id]);
    });
  }
}
