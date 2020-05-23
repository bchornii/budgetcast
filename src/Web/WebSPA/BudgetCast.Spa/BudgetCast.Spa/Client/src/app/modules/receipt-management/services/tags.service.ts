import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base-data.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ConfigurationService } from 'src/app/services/configuration-service';

@Injectable({
  providedIn: 'root'
})
export class TagsService extends BaseService {

  constructor(private httpClient: HttpClient,
              private configService: ConfigurationService) {
    super();
  }

  search(term?: string, amount: number = 10): Observable<string[]> {
    let params = new HttpParams();

    if (amount) {
      params = params.set('amount', amount.toString());
    }

    if (term) {
      params = params.set('term', term);
    }

    return this.httpClient.get<string[]>(
      `${this.configService.endpoints.dashboard.tags.search}`, { params }).pipe(
      catchError(this.handleError)
    );
  }
}
