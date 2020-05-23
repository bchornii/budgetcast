import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddReceiptComponent } from './pages/add-receipt/add-receipt.component';
import { ReceiptManagementRoutingModule } from './receipt-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReceiptDashboardComponent } from './pages/receipt-dashboard/receipt-dashboard.component';
import { ReceiptManagementComponent } from './pages/receipt-management/receipt-management.component';
import { ReceiptCardComponent } from './components/receipt-card/receipt-card.component';
import { CampaignTotalsComponent } from './components/campaign-totals/campaign-totals.component';
import { ReceiptDetailsComponent } from './pages/receipt-details/receipt-details.component';

@NgModule({
  declarations: [
    AddReceiptComponent,
    ReceiptDashboardComponent,
    ReceiptManagementComponent,
    ReceiptCardComponent,
    CampaignTotalsComponent,
    ReceiptDetailsComponent
  ],
  entryComponents: [
    CampaignTotalsComponent
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
