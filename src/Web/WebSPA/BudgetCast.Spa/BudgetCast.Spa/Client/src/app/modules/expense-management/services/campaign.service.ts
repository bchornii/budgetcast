import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base-data.service';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { KeyValuePair } from 'src/app/util/util';
import { ConfigurationService } from 'src/app/services/configuration-service';
import { CampaignVm } from '../pages/models/campaign-vm';
import { TotalsPerCampaignVm } from '../pages/models/totals-per-campaign-vm';

@Injectable({
  providedIn: 'root'
})
export class CampaignService extends BaseService {

  constructor(private httpClient: HttpClient,
              private configService: ConfigurationService) {
    super();
   }

  getAllCampaigns(): Observable<CampaignVm[]> {
    return this.httpClient.get<CampaignVm[]>(
      `${this.configService.endpoints.expenses.campaign.all}`).pipe(
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
      `${this.configService.endpoints.expenses.campaign.search}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  getTotals(campaign: string) : Observable<TotalsPerCampaignVm>{
    return this.httpClient.get<TotalsPerCampaignVm>(
      this.configService.endpoints.expenses.campaign.totals.replace('{{name}}', campaign)).pipe(
        catchError(this.handleError)
      );
  }
}
