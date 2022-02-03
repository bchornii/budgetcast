import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { BasicNotification } from '../models/notifications/basic-notification-vm';
import { AuthService } from './auth.service';
import { ConfigurationService } from './configuration-service';
import { LocalStorageService } from './local-storage.service';
import { SignalRService } from './signal-r.service';
import { signalRConnectionOptions } from "./signalRConnectionOptions";

@Injectable({
  providedIn: 'root'
})
export class NotificationsService extends SignalRService {

  private _connection: signalR.HubConnection;

  constructor(private authService: AuthService,
              private configurationService: ConfigurationService,
              localStorage: LocalStorageService,
              toastrService: ToastrService) { 
    super(localStorage, toastrService);
  }

  async startConnection() {
    let options = new signalRConnectionOptions()
      .withUri(this.configurationService.endpoints.notifications.all)
      .withReconnectOnConnectionDropPredicate(() => this.authService.isUserValid);    

    this._connection = this.createConnection(options);

    await this.start(this._connection);
  }

  async stopConnection() {
    await this.stop(this._connection); 
  }

  addNotificationsListener() {
    this._connection.on("BasicNotification", (notification: BasicNotification) => {
      this.toastrService.success(notification.message, notification.messageType);
    });
  }
}
