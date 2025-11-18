import { Routes } from '@angular/router';
import { UnassignedLoans } from './unassigned-loans/unassigned-loans';
import { AssignedLoans } from './assigned-loans/assigned-loans';
import { AnalystLoanDetail } from './analyst-loan-detail/analyst-loan-detail';
import { AnalystHome } from './analyst-home/analyst-home';

export const ANALYST_ROUTES: Routes = [
  {
    path: '',
    component: AnalystHome
  },
  { path: 'unassigned', component: UnassignedLoans },
  { path: 'assigned', component: AssignedLoans },
  { path: 'loan/:id', component: AnalystLoanDetail }
];
