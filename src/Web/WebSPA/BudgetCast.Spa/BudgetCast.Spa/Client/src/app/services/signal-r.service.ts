import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { AccessTokenItem } from '../util/constants/auth-constants';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  constructor(private localStorage: LocalStorageService) {
  }

  createConnection(hubUri: string, logLevel: signalR.LogLevel, 
    withAutoReconnect: boolean = true, 
    withAuth: boolean = true,
    customReconnectDelays: number[] = null): signalR.HubConnection {
    
    let uri = withAuth
      ? this.getUriWithAccessToken(hubUri)
      : hubUri;

    let builder = new signalR.HubConnectionBuilder()
      .withUrl(uri)
      .configureLogging(logLevel);

    if (withAutoReconnect && customReconnectDelays) {
      builder.withAutomaticReconnect(customReconnectDelays);
    } else {
      builder.withAutomaticReconnect();
    }

    return builder.build();
  }

  private getUriWithAccessToken(hubUri: string) {
    const token: string = this.localStorage.getItem(AccessTokenItem);
    if (token) {
      return `${hubUri}?access_token=${token}`;
    }
    return hubUri;
  }
}
