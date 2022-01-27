import { Routes, RouterModule } from '@angular/router';
import { AddExpenseComponent } from './pages/add-expense/add-expense.component';
import { ExpenseDashboardComponent } from './pages/expenses-dashboard/expense-dashboard.component';
import { NgModule } from '@angular/core';
import { ExpenseManagementComponent } from './pages/expense-management/expense-management.component';
import { ExpenseDetailsComponent } from './pages/expense-details/expense-details.component';
import { ExpenseDetailsResolver } from './resolvers/expense-items.resolver';

export const routes: Routes = [
  {
    path: '',
    component: ExpenseManagementComponent,
    children: [
      {
        path: 'dashboard',
        component: ExpenseDashboardComponent
      },
      {
        path: 'add-expense',
        component: AddExpenseComponent
      },
      {
        path: 'expense-details/:id',
        component: ExpenseDetailsComponent,
        resolve: {
          expense: ExpenseDetailsResolver
        }
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  }
];
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class ExpenseManagementRoutingModule {}
