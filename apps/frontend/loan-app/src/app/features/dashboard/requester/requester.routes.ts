import { Routes } from '@angular/router';
import { MyLoans } from './my-loans/my-loans';
import { NewLoan } from './new-loan/new-loan';
import { RequesterHome } from './requester-home/requester-home';

export const REQUESTER_ROUTES: Routes = [
  {
    path: '',
    component: RequesterHome // Default route
  },
  { path: 'my-loans', component: MyLoans },
  { path: 'new-loan', component: NewLoan }
];
