export interface LoanBrief {
  id: string;
  requestedAmount: number;
  currentBalance: number;
  statusName: string;
  requestedDate: string;
  analystFullName: string;
  requesterFullName: string;
}

export interface Payment {
  id: string;
  amountPaid: number;
  paymentDate: string;
  processedBy: string;
}

export interface LoanHistory {
  id: string;
  comment: string;
  timestamp: string;
  oldStatus: string;
  newStatus: string;
}

export interface LoanDetails {
  id: string;
  requestedAmount: number;
  currentBalance: number;
  termInMonths: number;
  purpose: string;
  requestedDate: string;
  decisionDate?: string;
  statusName: string;
  requesterFullName: string;
  analystFullName: string;
  history: LoanHistory[];
  payments: Payment[];
}
