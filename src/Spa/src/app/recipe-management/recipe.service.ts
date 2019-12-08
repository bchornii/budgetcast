import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService } from '../shared/services/base-data.service';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RecipeService extends BaseService {

  constructor(private httpClient: HttpClient) {
    super();
  }

  getCategories(term?: string, amount: number = 10): Observable<string[]> {
    let params = new HttpParams();

    if (amount) {
      params = params.set('amount', amount.toString());
    }

    if (term) {
      params = params.set('name', term);
    }

    return this.httpClient.get<string[]>(
      `${environment.api.recipesApi.categories}`, { params }).pipe(
      catchError(this.handleError)
    );
  }
}
