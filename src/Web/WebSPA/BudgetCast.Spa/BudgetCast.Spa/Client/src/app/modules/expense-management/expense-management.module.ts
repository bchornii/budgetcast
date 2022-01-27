import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddExpenseComponent } from './pages/add-expense/add-expense.component';
import { ExpenseManagementRoutingModule } from './expense-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ExpenseDashboardComponent } from './pages/expenses-dashboard/expense-dashboard.component';
import { ExpenseManagementComponent } from './pages/expense-management/expense-management.component';
import { ExpenseCardComponent } from './components/expense-card/expense-card.component';
import { CampaignTotalsComponent } from './components/campaign-totals/campaign-totals.component';
import { ExpenseDetailsComponent } from './pages/expense-details/expense-details.component';

@NgModule({
    declarations: [
        AddExpenseComponent,
        ExpenseDashboardComponent,
        ExpenseManagementComponent,
        ExpenseCardComponent,
        CampaignTotalsComponent,
        ExpenseDetailsComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ExpenseManagementRoutingModule,
        SharedModule,
    ]
})
export class ExpenseManagementModule { }
