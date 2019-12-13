import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddReceiptComponent } from './add-receipt/add-receipt.component';
import { RouterModule } from '@angular/router';
import { RecipeManagementRoutes } from './receipt-management.routes';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { ReceiptDashboardComponent } from './receipt-dashboard/receipt-dashboard.component';

@NgModule({
  declarations: [
    AddReceiptComponent,
    ReceiptDashboardComponent
  ],
  imports: [
    CommonModule,
    FormsModule,

    RouterModule.forChild(RecipeManagementRoutes),

    SharedModule,
  ]
})
export class RecipeManagementModule { }
