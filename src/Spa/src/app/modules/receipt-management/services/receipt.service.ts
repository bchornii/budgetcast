import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { catchError } from 'rxjs/operators';
import { BaseService } from 'src/app/services/base-data.service';
import { AddBasicReceipt } from '../pages/models/add-receipt';
import { PageResult } from 'src/app/models/page-result';
import { BasicReceipt } from '../pages/models/basic-receipt';
import { TotalsPerCampaign } from '../pages/models/totals-per-campaign';

@Injectable({
  providedIn: 'root'
})
export class RecipeService extends BaseService {

  constructor(private httpClient: HttpClient) {
    super();
  }

  getTags(term?: string, amount: number = 10): Observable<string[]> {
    let params = new HttpParams();

    if (amount) {
      params = params.set('amount', amount.toString());
    }

    if (term) {
      params = params.set('term', term);
    }

    return this.httpClient.get<string[]>(
      `${environment.api.recipesApi.tags}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  getBasicReceipts(campaign: string, page = 1, pageSize = 10): Observable<PageResult<BasicReceipt>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if(campaign) {
      params = params.set('campaignName', campaign);
    }    

    return this.httpClient.get<PageResult<BasicReceipt>>(
      `${environment.api.recipesApi.basicReceipts}`, {params}).pipe(
        catchError(this.handleError)
      );
  }

  getTotalsPerCampaign(campaign: string) : Observable<TotalsPerCampaign>{
    return this.httpClient.get<TotalsPerCampaign>(
      environment.api.recipesApi.totalPerCampaign.replace('{{campaignName}}', campaign)).pipe(
        catchError(this.handleError)
      );
  }

  addBasicReceipt(receipt: AddBasicReceipt): Observable<any> {
    return this.httpClient.post(
      `${environment.api.recipesApi.addBasic}`, receipt).pipe(
        catchError(this.handleError)
      );
  }
}
