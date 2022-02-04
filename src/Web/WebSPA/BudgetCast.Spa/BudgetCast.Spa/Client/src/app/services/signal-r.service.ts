import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { AccessTokenItem } from '../util/constants/auth-constants';
import { LocalStorageService } from './local-storage.service';
import { signalRConnectionOptions } from './signalRConnectionOptions';

export class SignalRService {

  private delayBeteweenReconnectOnStart: number = 3000;

  constructor(protected localStorage: LocalStorageService,
              protected toastrService: ToastrService) {
  }

  protected createConnection(options: signalRConnectionOptions): signalR.HubConnection {
    
    let {       
      hubUri,
      logLevel,
      withAutoReconnect,
      withAuth,
      withDefaultOnStateChangeActions,
      customReconnectDelays,
      reconnectOnConnectionDropPredicate
    } = options;

    let uri = withAuth
      ? this.getUriWithAccessToken(hubUri)
      : hubUri;
    let validatedLogLevel = this.validateLogLevel(logLevel);

    let builder = new signalR.HubConnectionBuilder()
      .withUrl(uri)
      .configureLogging(validatedLogLevel);

    if (withAutoReconnect && customReconnectDelays) {
      builder.withAutomaticReconnect(customReconnectDelays);
    } else {
      builder.withAutomaticReconnect();
    }

    if (withDefaultOnStateChangeActions) {
      let connection = builder.build();

      connection.onreconnecting(() => {
        this.toastrService.warning(
          'Reconnecting to the server... Some notifications might be missing until done.', 
          'Notifications hub'
        );
      });
  
      connection.onreconnected(() => {
        this.toastrService.success(
          'Reconnected to the server.', 
          'Notifications hub'
        );
      });

      connection.onclose(async () => {      
        if(reconnectOnConnectionDropPredicate()) {
          this.toastrService.error(
            'Server connection is distrupted.',
            'Notifications hub'
          );
          await this.start(connection);
        }      
      });

      return connection;      
    } else {
      return builder.build();
    }    
  }

  protected async start(connection: signalR.HubConnection) {
    try {
      await connection.start();
    } catch(err) {
      console.log('Error while starting the connection: ' + err);
      setTimeout(() => {
        this.start(connection);
      }, this.delayBeteweenReconnectOnStart);
    }
  }

  protected async stop(connection: signalR.HubConnection) {
    let isActive = connection &&
                  (connection.state == signalR.HubConnectionState.Connected ||
                   connection.state == signalR.HubConnectionState.Connecting ||
                   connection.state == signalR.HubConnectionState.Reconnecting);

    if(isActive) {
      await connection.stop();
    }
  }

  private getUriWithAccessToken(hubUri: string) {
    const token: string = this.localStorage.getItem(AccessTokenItem);
    if (token) {
      return `${hubUri}?access_token=${token}`;
    }
    return hubUri;
  }

  private validateLogLevel(userSelectedLogLevel: signalR.LogLevel = null) {
    if (environment.production) {
      return signalR.LogLevel.Error;
    }

    return userSelectedLogLevel || signalR.LogLevel.Information;
  }
}


