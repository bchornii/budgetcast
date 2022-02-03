import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { BasicNotification } from '../models/notifications/basic-notification-vm';
import { AuthService } from './auth.service';
import { ConfigurationService } from './configuration-service';
import { SignalRService } from './signal-r.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  private hubConnection: signalR.HubConnection;

  constructor(private toastr: ToastrService,
              private signalr: SignalRService,
              private authService: AuthService,
              private configurationService: ConfigurationService) { }

  async startConnection() {
    let logLevel = this.getLogLevel();
    this.hubConnection = this.signalr
      .createConnection(this.configurationService.endpoints.notifications.all, logLevel);

    this.hubConnection.onreconnecting(() => {
      this.toastr.warning(
        'Reconnecting to the server... Some notifications might be missing until done.', 
        'Notifications hub'
      );
    });

    this.hubConnection.onreconnected(() => {
      this.toastr.success(
        'Reconnected to the server.', 
        'Notifications hub'
      );
    });

    this.hubConnection.onclose(async () => {      
      if(this.authService.isUserValid) {
        this.toastr.error(
          'Server connection is distrupted.',
          'Notifications hub'
        );
        await this.start();
      }      
    });

    await this.start();
  }

  async stopConnection() {
    if (this.isHubConnectionActive()){
      await this.hubConnection.stop();
    }    
  }

  addNotificationsListener() {
    this.hubConnection.on("BasicNotification", (notification: BasicNotification) => {
      this.toastr.success(notification.message, notification.messageType);
    });
  }

  private async start() {
    try {
      await this.hubConnection.start();
    } catch(err) {
      console.log('error which starting the connection: ' + err);
      setTimeout(() => {
        this.start();
      }, 3000);
    }
  }

  private getLogLevel() {
    if (environment.production) {
      return signalR.LogLevel.Error;
    }
    return signalR.LogLevel.Information;
  }

  private isHubConnectionActive(){
    return this.hubConnection &&
           (this.hubConnection.state == signalR.HubConnectionState.Connected ||
            this.hubConnection.state == signalR.HubConnectionState.Connecting ||
            this.hubConnection.state == signalR.HubConnectionState.Reconnecting);
  }
}
