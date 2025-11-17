import { Routes } from '@angular/router';
import { UnassignedLoans } from './unassigned-loans/unassigned-loans';
import { AssignedLoans } from './assigned-loans/assigned-loans';
import { AnalystLoanDetail } from './analyst-loan-detail/analyst-loan-detail';

export const ANALYST_ROUTES: Routes = [
  { path: 'unassigned', component: UnassignedLoans },
  { path: 'assigned', component: AssignedLoans },
  { path: 'loan/:id', component: AnalystLoanDetail },
  { path: '', redirectTo: 'unassigned', pathMatch: 'full' }
];
