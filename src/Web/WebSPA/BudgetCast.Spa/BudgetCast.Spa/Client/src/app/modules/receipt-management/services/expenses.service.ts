import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PageResult } from 'src/app/models/page-result';
import { BaseService } from 'src/app/services/base-data.service';
import { ConfigurationService } from 'src/app/services/configuration-service';
import { AddExpenseDto } from '../pages/models/add-expense-dto';
import { ExpenseDetailsVm } from '../pages/models/expense-details-vm';
import { ExpenseVm } from '../pages/models/expense-vm';

@Injectable({
  providedIn: 'root'
})
export class ExpensesService extends BaseService {

  constructor(private httpClient: HttpClient,
              private configService: ConfigurationService) { 
    super();
  }

  searchTags(term?: string, amount: number = 10): Observable<string[]> {
    let params = new HttpParams();

    if(amount) {
      params = params.set('amount', amount.toString());
    }

    if(term) {
      params = params.set('term', term);
    }

    return this.httpClient.get<string[]>(
      `${this.configService.endpoints.expenses.expenses.searchTags}`, { params }).pipe(
        catchError(this.handleError)
      );
  }

  addExpense(expense: AddExpenseDto): Observable<any> {
    return this.httpClient.post(
      `${this.configService.endpoints.expenses.expenses.add}`, expense).pipe(
        catchError(this.handleError)
      );
  }

  getExpenses(campaign: string, page = 1, pageSize = 10): Observable<PageResult<ExpenseVm>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if(campaign) {
      params = params.set('campaignName', campaign);
    }

    return this.httpClient.get<PageResult<ExpenseVm>>(
      `${this.configService.endpoints.expenses.expenses.get}`, {params}).pipe(
        catchError(this.handleError)
      );
  }

  getExpenseDetails(id: number) : Observable<ExpenseDetailsVm> {
    return this.httpClient.get<ExpenseDetailsVm>(
      this.configService.endpoints.expenses.expenses.details.replace('{{id}}', id.toString())).pipe(
        catchError(this.handleError)
    );
  }
}
