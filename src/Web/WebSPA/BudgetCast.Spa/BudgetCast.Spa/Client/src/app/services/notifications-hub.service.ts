import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { BasicNotification } from '../models/notifications/basic-notification-vm';
import { NotificationType } from '../models/notifications/notification-type-vm';
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
      this.showNotification(notification.type, notification.message);
    });
  }

  private showNotification(type: NotificationType, message: string, title: string = 'Notification') {
    if(type == NotificationType.Success){
      this.toastrService.success(message, title);
    }
    if(type == NotificationType.Error) {
      this.toastrService.error(message, title);
    }
    if(type == NotificationType.Information ||
       type == NotificationType.Unknown) {
      this.toastrService.info(message, title);
    }
    if(type == NotificationType.Warning) {
      this.toastrService.warning(message, title);
    }
  }
}
