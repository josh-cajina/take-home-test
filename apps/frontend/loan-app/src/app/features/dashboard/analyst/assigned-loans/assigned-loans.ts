import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { LoanBrief } from '../../../../core/models/loan';
import { LoanService } from '../../../../core/services/loan';

@Component({
  selector: 'app-assigned-loans',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './assigned-loans.html',
  styleUrl: './assigned-loans.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssignedLoans implements OnInit {
  private loanService = inject(LoanService);
  loans = signal<LoanBrief[]>([]);

  ngOnInit() {
    this.loanService.getAllLoans().pipe(
      map(loans => loans.filter(l => l.analystFullName !== 'Unassigned'))
    ).subscribe(data => this.loans.set(data));
  }
}
