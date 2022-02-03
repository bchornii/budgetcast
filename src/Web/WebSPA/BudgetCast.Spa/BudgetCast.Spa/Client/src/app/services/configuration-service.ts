import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Subject, Observable, of } from 'rxjs';
import { IConfiguration } from 'src/app/models/configuration-vm';
import { tap } from 'rxjs/operators';
import { Endpoints } from '../util/constants/api-endpoints';

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
        identity: this.getIdentityEndpoints(endpointsConfig.endpoints.identity),
        expenses: this.getExpensesEndpoints(endpointsConfig.endpoints.expenses),
        notifications: this.getNotificationsEndpoints(endpointsConfig.endpoints.notifications)
      };
    }

    private getExpensesEndpoints(baseUrl) {
      return {
        campaign: {
          all: `${baseUrl}/campaigns`,
          search: `${baseUrl}/campaigns/search`,
          totals: `${baseUrl}/campaigns/{{name}}/totals`
        },
        expenses: {
          get: `${baseUrl}/expenses`,
          add: `${baseUrl}/expenses`,
          searchTags: `${baseUrl}/expenses/tags/search`,
          details: `${baseUrl}/expenses/{{id}}`
        }     
      };
    }

    private getIdentityEndpoints(baseUrl) {
      return {
        account: {
          register: `${baseUrl}/account/register`,
          update: `${baseUrl}/account/update`,
          emailConfirmation: `${baseUrl}/account/email/confirm`,
          passwordForgot: `${baseUrl}/account/password/forgot`,
          passwordReset: `${baseUrl}/account/password/reset`,
          isAuthenticated: `${baseUrl}/account/isAuthenticated`
        },
    
        signIn: {
          google: `${baseUrl}/signin/google`,
          facebook: `${baseUrl}/signin/facebook`,
          individual: `${baseUrl}/signin/individual`
        },
    
        signOut: {
          all: `${baseUrl}/signout`
        }
      };
    }

    private getNotificationsEndpoints(baseUrl) {
      return {
        all: `${baseUrl}/notifications`
      }
    }
}
