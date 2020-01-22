import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddReceiptComponent } from './pages/add-receipt/add-receipt.component';
import { ReceiptManagementRoutingModule } from './receipt-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReceiptDashboardComponent } from './pages/receipt-dashboard/receipt-dashboard.component';
import { ReceiptManagementComponent } from './pages/receipt-management/receipt-management.component';
import { ReceiptCardComponent } from './components/receipt-card/receipt-card.component';

@NgModule({
  declarations: [
    AddReceiptComponent,
    ReceiptDashboardComponent,
    ReceiptManagementComponent,
    ReceiptCardComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ReceiptManagementRoutingModule,

    SharedModule,
  ]
})
export class RecipeManagementModule { }
