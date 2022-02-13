import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { NotificationType } from '../models/notifications/notification-type-vm';
import { AuthService } from './auth.service';
import { ConfigurationService } from './configuration-service';
import { StorageService } from './storage.service';
import { SignalRService } from './signal-r.service';
import { signalRConnectionOptions } from "./signalRConnectionOptions";
import { NotificationBase } from '../models/notifications/notification-base.vm';
import { GeneralNotification } from '../models/notifications/general-notification-vm';
import { ChannelsService } from '../channels/channels.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService extends SignalRService {

  private _connection: signalR.HubConnection;

  constructor(private configurationService: ConfigurationService,
              private channelsService: ChannelsService,
              storageService: StorageService,
              toastrService: ToastrService,
              authService: AuthService) { 
    super(storageService, toastrService, authService);
  }

  initializeConnection(): NotificationsService {
    let options = new signalRConnectionOptions()
      .withUri(this.configurationService.endpoints.notifications.all)
      .withReconnectOnConnectionDropPredicate(() => this.authService.isUserValid);    // TODO: maybe call isAuthenticated ?

    this._connection = this.createConnection(options);

    return this;
  }

  addListeners(): NotificationsService {
    this.addNotificationsListener();
    return this;
  }

  async startCommunication() {
    await this.start(this._connection);
  }

  async stopCommunication() {
    await this.stop(this._connection); 
  }

  private addNotificationsListener() {
    this._connection.on("GeneralNotification", (notification: GeneralNotification) => {
      this.showNotification(notification);
      this.channelsService.write(notification);      
    });
  }

  private showNotification(notification: NotificationBase, title: string = 'Notification') {

    let { type, message } = notification;

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
