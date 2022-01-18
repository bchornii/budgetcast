import { Routes, RouterModule } from '@angular/router';
import { AddReceiptComponent } from './pages/add-receipt/add-receipt.component';
import { ReceiptDashboardComponent } from './pages/receipt-dashboard/receipt-dashboard.component';
import { NgModule } from '@angular/core';
import { ReceiptManagementComponent } from './pages/receipt-management/receipt-management.component';
import { ReceiptDetailsComponent } from './pages/receipt-details/receipt-details.component';
import { ReceiptDetailsResolver } from './resolvers/receipt-items.resolver';

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
