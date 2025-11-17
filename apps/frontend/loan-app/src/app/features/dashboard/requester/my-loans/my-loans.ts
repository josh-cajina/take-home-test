import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoanService } from '../../../../core/services/loan';
import { LoanBrief } from '../../../../core/models/loan';

@Component({
  selector: 'app-my-loans',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-loans.html',
  styleUrl: './my-loans.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyLoans implements OnInit {
  private loanService = inject(LoanService);
  loans = signal<LoanBrief[]>([]);

  ngOnInit() {
    this.loanService.getAllLoans().subscribe(data => this.loans.set(data));
  }
}
