import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base-data.service';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { KeyValuePair } from 'src/app/util/util';

@Injectable({
  providedIn: 'root'
})
export class CampaignService extends BaseService {

  constructor(private httpClient: HttpClient) {
    super();
   }

  getAllCampaigns(): Observable<KeyValuePair[]> {
    return this.httpClient.get<KeyValuePair[]>(
      `${environment.api.campaignsApi.all}`).pipe(      
        catchError(this.handleError)
    );
  }

  
  getCampaigns(term?: string, amount: number = 10): Observable<string[]> {
    let params = new HttpParams();

    if (amount) {
      params = params.set('amount', amount.toString());
    }

    if (term) {
      params = params.set('term', term);
    }

    return this.httpClient.get<string[]>(
      `${environment.api.campaignsApi.search}`, { params }).pipe(
      catchError(this.handleError)
    );
  }
}
