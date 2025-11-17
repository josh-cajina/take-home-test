import { Routes } from '@angular/router';
import { MyLoans } from './my-loans/my-loans';
import { NewLoan } from './new-loan/new-loan';

export const REQUESTER_ROUTES: Routes = [
  { path: 'my-loans', component: MyLoans },
  { path: 'new-loan', component: NewLoan },
  { path: '', redirectTo: 'my-loans', pathMatch: 'full' }
];
