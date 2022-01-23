import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { ExpensesService } from '../services/expenses.service';
import { ExpenseDetailsVm } from '../pages/models/expense-details-vm';

@Injectable({
    providedIn: 'root'
})
export class ExpenseDetailsResolver implements Resolve<Observable<ExpenseDetailsVm>> {

    constructor(private expenseService: ExpensesService) {}

    resolve(route: ActivatedRouteSnapshot) : Observable<ExpenseDetailsVm> {
        return this.expenseService.getExpenseDetails(route.params['id']);
    }
}