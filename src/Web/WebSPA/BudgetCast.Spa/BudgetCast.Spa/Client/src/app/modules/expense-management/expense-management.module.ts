import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddReceiptComponent } from './pages/add-expense/add-receipt.component';
import { ReceiptManagementRoutingModule } from './expense-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReceiptDashboardComponent } from './pages/expenses-dashboard/receipt-dashboard.component';
import { ReceiptManagementComponent } from './pages/expense-management/receipt-management.component';
import { ReceiptCardComponent } from './components/expense-card/receipt-card.component';
import { CampaignTotalsComponent } from './components/campaign-totals/campaign-totals.component';
import { ReceiptDetailsComponent } from './pages/expense-details/receipt-details.component';

@NgModule({
    declarations: [
        AddReceiptComponent,
        ReceiptDashboardComponent,
        ReceiptManagementComponent,
        ReceiptCardComponent,
        CampaignTotalsComponent,
        ReceiptDetailsComponent
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
