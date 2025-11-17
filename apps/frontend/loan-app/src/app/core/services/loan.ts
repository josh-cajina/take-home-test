import { Injectable, inject } from '@angular/core';
import { ApiService } from './api';
import { LoanBrief, LoanDetails } from '../models/loan';

@Injectable({ providedIn: 'root' })
export class LoanService {
  private api = inject(ApiService);

  getAllLoans() { return this.api.get<LoanBrief[]>('loans'); }
  getLoanById(id: string) { return this.api.get<LoanDetails>(`loans/${id}`); }
  createLoan(data: any) { return this.api.post<string>('loans', data); }
  updateStatus(id: string, data: any) { return this.api.put<void>(`loans/${id}/status`, data); }
  addPayment(id: string, amount: number) { return this.api.post<string>(`loans/${id}/payments`, { loanId: id, amount }); }
}
