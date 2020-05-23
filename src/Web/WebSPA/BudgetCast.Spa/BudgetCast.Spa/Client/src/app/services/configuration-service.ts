import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Subject, Observable } from 'rxjs';
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

    load(): Observable<IConfiguration> {

        const baseURI = environment.production
          ? this.getBaseUri()
          : environment.devBaseUrl;

        return this.http.get<IConfiguration>(baseURI).pipe(tap(response => {

            console.log('server settings loaded');
            console.log(response);

            const config = (response as IConfiguration).endpoints;
            this.endpoints = this.getEndpoints(config.dashboard);
            this.isReady = true;
            this.settingsLoadedSource.next();
        }));
    }

    private getEndpoints(dashboardUrl: string): Endpoints {
      return {
        dashboard: this.getDashBoardEndpoints(dashboardUrl)
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


        campaign: {
            all: `${baseUrl}/campaigns`,
            search: `${baseUrl}/campaigns/search`
        },

        tags: {
            search: `${baseUrl}/tags/search`
        }
      };
    }

    private getBaseUri() {
      const baseURI = document.baseURI.endsWith('/')
        ? document.baseURI : `${document.baseURI}/`;
      return `${baseURI}api/Configs/endpoints`;
    }
}
