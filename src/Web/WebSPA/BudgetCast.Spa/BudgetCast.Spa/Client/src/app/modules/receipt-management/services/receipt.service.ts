import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BaseService } from 'src/app/services/base-data.service';
import { Receipt } from '../pages/models/receipt';
import { ConfigurationService } from 'src/app/services/configuration-service';

@Injectable({
  providedIn: 'root'
})
export class RecipeService extends BaseService {

  constructor(private httpClient: HttpClient,
              private configService: ConfigurationService) {
    super();
  }

  getReceiptDetails(id: string) : Observable<Receipt> {
    return this.httpClient.get<Receipt>(
      this.configService.endpoints.dashboard.receipt.details.replace('{{id}}', id)).pipe(
        catchError(this.handleError)
    );
  }
}
