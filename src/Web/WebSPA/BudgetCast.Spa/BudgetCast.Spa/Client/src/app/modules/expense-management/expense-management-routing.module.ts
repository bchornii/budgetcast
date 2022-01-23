import { Routes, RouterModule } from '@angular/router';
import { AddReceiptComponent } from './pages/add-expense/add-receipt.component';
import { ReceiptDashboardComponent } from './pages/expenses-dashboard/receipt-dashboard.component';
import { NgModule } from '@angular/core';
import { ReceiptManagementComponent } from './pages/expense-management/receipt-management.component';
import { ReceiptDetailsComponent } from './pages/expense-details/receipt-details.component';
import { ReceiptDetailsResolver } from './resolvers/expense-items.resolver';

export const routes: Routes = [
  {
    path: '',
    component: ReceiptManagementComponent,
    children: [
      {
        path: 'dashboard',
        component: ReceiptDashboardComponent
      },
      {
        path: 'add-receipt',
        component: AddReceiptComponent
      },
      {
        path: 'receipt-details/:id',
        component: ReceiptDetailsComponent,
        resolve: {
          expense: ReceiptDetailsResolver
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
export class ReceiptManagementRoutingModule {}
