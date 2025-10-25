import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Endpoints } from '../../constants/api-endpoints';
import { BaseService } from '../base-service';
import { IConfiguration } from './models/configuration-vm';

@Injectable({
  providedIn: 'root',
})
export class Configuration extends BaseService {
  private readonly retryCount = 2;
  private readonly retryDelay = 500;

  private settingsLoadedSource = new Subject<void>();
  private http: HttpClient = inject(HttpClient);

  endpoints: Endpoints = {} as Endpoints;
  settingsLoaded$ = this.settingsLoadedSource.asObservable();
  isReady = false;

  load(url: string): Observable<IConfiguration> {
    const request = this.http.get<IConfiguration>(url).pipe(
      tap((response) => {
        this.log('info', 'Server settings loaded');
        this.log('info', JSON.stringify(response));

        const endpointsConfig = response as IConfiguration;
        this.endpoints = this.getEndpoints(endpointsConfig);
        this.isReady = true;
        this.settingsLoadedSource.next(undefined);
      }),
      this.retryRequest(this.retryCount, this.retryDelay),
    );
    return this.executeRequest(request, 'Loading configuration', url);
  }

  private getEndpoints(endpointsConfig: IConfiguration): Endpoints {
    return {
      identity: this.getIdentityEndpoints(endpointsConfig.endpoints.identity),
      expenses: this.getExpensesEndpoints(endpointsConfig.endpoints.expenses),
      notifications: this.getNotificationsEndpoints(endpointsConfig.endpoints.notifications),
    };
  }

  private getExpensesEndpoints(baseUrl: string) {
    return {
      campaign: {
        all: `${baseUrl}/campaigns`,
        search: `${baseUrl}/campaigns/search`,
        totals: `${baseUrl}/campaigns/{{name}}/totals`,
      },
      expenses: {
        get: `${baseUrl}/expenses`,
        add: `${baseUrl}/expenses`,
        searchTags: `${baseUrl}/expenses/tags/search`,
        details: `${baseUrl}/expenses/{{id}}`,
      },
    };
  }

  private getIdentityEndpoints(baseUrl: string) {
    return {
      account: {
        register: `${baseUrl}/account/register`,
        update: `${baseUrl}/account/update`,
        emailConfirmation: `${baseUrl}/account/email/confirm`,
        passwordForgot: `${baseUrl}/account/password/forgot`,
        passwordReset: `${baseUrl}/account/password/reset`,
        isAuthenticated: `${baseUrl}/account/isAuthenticated`,
      },

      signIn: {
        google: `${baseUrl}/signin/google`,
        facebook: `${baseUrl}/signin/facebook`,
        individual: `${baseUrl}/signin/individual`,
        refreshAccessToken: `${baseUrl}/signin/refresh`,
      },

      signOut: {
        all: `${baseUrl}/signout`,
      },
    };
  }

  private getNotificationsEndpoints(baseUrl: string) {
    return {
      all: `${baseUrl}/notifications`,
    };
  }
}
