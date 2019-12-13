import { Routes } from '@angular/router';
import { AddReceiptComponent } from './add-receipt/add-receipt.component';
import { ReceiptDashboardComponent } from './receipt-dashboard/receipt-dashboard.component';
import { ReceiptDashboardGuard } from './services/receipt-dashboard.guard';

export const RecipeManagementRoutes: Routes = [
  {
    path: 'add-receipt',
    component: AddReceiptComponent
  },
  {
    path: 'receipt-dashboard',
    component: ReceiptDashboardComponent,
    canActivate: [ReceiptDashboardGuard]
  },
  {
    path: '',
    redirectTo: 'receipt-dashboard',
    pathMatch: 'full'
  }
];
