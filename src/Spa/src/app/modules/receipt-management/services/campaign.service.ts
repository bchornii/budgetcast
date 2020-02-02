import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base-data.service';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { KeyValuePair } from 'src/app/util/util';
import { dashboard } from 'src/app/util/constants/api-endpoints';

@Injectable({
  providedIn: 'root'
})
export class CampaignService extends BaseService {

  constructor(private httpClient: HttpClient) {
    super();
   }

  getAllCampaigns(): Observable<KeyValuePair[]> {
    return this.httpClient.get<KeyValuePair[]>(
      `${dashboard.campaign.all}`).pipe(      
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
      `${dashboard.campaign.search}`, { params }).pipe(
      catchError(this.handleError)
    );
  }
}
