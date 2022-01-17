import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Subject, Observable, of } from 'rxjs';
import { IConfiguration } from 'src/app/models/configuration';
import { tap } from 'rxjs/operators';
import { Endpoints } from '../util/constants/api-endpoints';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

    private settingsLoadedSource = new Subject();

    endpoints: Endpoints;
    settingsLoaded$ = this.settingsLoadedSource.asObservable();
    isReady = false;

    constructor(private http: HttpClient) { }

    load(url: string): Observable<IConfiguration> {

        return this.http.get<IConfiguration>(url).pipe(tap(response => {
        
            console.log('server settings loaded');
            console.log(response);
        
            const endpointsConfig = (response as IConfiguration);
            this.endpoints = this.getEndpoints(endpointsConfig);
            this.isReady = true;
            this.settingsLoadedSource.next();
        }));
    }

    private getEndpoints(endpointsConfig: IConfiguration): Endpoints {
      return {
        dashboard: this.getDashBoardEndpoints(endpointsConfig.endpoints.dashboard),
        expenses: this.getExpensesEndpoints(endpointsConfig.endpoints.expenses)
      };
    }

    private getDashBoardEndpoints(baseUrl: string) {
      return {
        account: {
            isAuthenticated: `${baseUrl}/account/isAuthenticated`,
            signInWithGoogle: `${baseUrl}/account/signInWithGoogle`,
            signInWithFacebook: `${baseUrl}/account/signInWithFacebook`,
            login: `${baseUrl}/account/login`,
            logout: `${baseUrl}/account/logout`,
            check: `${baseUrl}/account/check`,
            updateProfile: `${baseUrl}/account/updateProfile`,
            register: `${baseUrl}/account/register`,
            forgotPassword: `${baseUrl}/account/forgotPassword`,
            resetPassword: `${baseUrl}/account/resetPassword`
        },

        receipt: {
            addBasic: `${baseUrl}/receipt/addBasic`,
            basicReceipts: `${baseUrl}/receipt/basicReceipts`,
            totalPerCampaign: `${baseUrl}/receipt/total/{{campaignName}}`,
            details: `${baseUrl}/receipt/{{id}}/details`
        },

        tags: {
            search: `${baseUrl}/tags/search`
        }
      };
    }

    private getExpensesEndpoints(baseUrl) {
      return {
        campaign: {
          all: `${baseUrl}/campaigns`,
          search: `${baseUrl}/campaigns/search`
        },
        expenses: {
          add: `${baseUrl}/expenses`,
          searchTags: `${baseUrl}/expenses/tags/search`
        }     
      };
    }
}
