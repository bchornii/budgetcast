import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BaseService } from 'src/app/services/base-data.service';
import { AddBasicReceipt } from '../pages/models/add-receipt';
import { PageResult } from 'src/app/models/page-result';
import { BasicReceipt } from '../pages/models/basic-receipt';
import { TotalsPerCampaign } from '../pages/models/totals-per-campaign';
import { dashboard } from 'src/app/util/constants/api-endpoints';
import { ReceiptItem } from '../pages/models/receipt-item';
import { Receipt } from '../pages/models/receipt';

@Injectable({
  providedIn: 'root'
})
export class RecipeService extends BaseService {

  constructor(private httpClient: HttpClient) {
    super();
  }

  getBasicReceipts(campaign: string, page = 1, pageSize = 10): Observable<PageResult<BasicReceipt>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if(campaign) {
      params = params.set('campaignName', campaign);
    }    

    return this.httpClient.get<PageResult<BasicReceipt>>(
      `${dashboard.receipt.basicReceipts}`, {params}).pipe(
        catchError(this.handleError)
      );
  }

  getTotalsPerCampaign(campaign: string) : Observable<TotalsPerCampaign>{
    return this.httpClient.get<TotalsPerCampaign>(
      dashboard.receipt.totalPerCampaign.replace('{{campaignName}}', campaign)).pipe(
        catchError(this.handleError)
      );
  }

  getReceiptDetails(id: string) : Observable<Receipt> {
    return this.httpClient.get<Receipt>(
      dashboard.receipt.details.replace('{{id}}', id)).pipe(
        catchError(this.handleError)
    );
  }

  addBasicReceipt(receipt: AddBasicReceipt): Observable<any> {
    return this.httpClient.post(
      `${dashboard.receipt.addBasic}`, receipt).pipe(
        catchError(this.handleError)
      );
  }
}
