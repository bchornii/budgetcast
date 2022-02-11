import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { AccessTokenItem } from '../util/constants/auth-constants';
import { AuthService } from "./auth.service";
import { LocalStorageService } from './local-storage.service';
import { signalRConnectionOptions } from './signalRConnectionOptions';

export class SignalRService {

  private delayBeteweenReconnectOnStart: number = 3000;
  private totalReconnectAttempts: number = 0;
  private maxReconnectAttempts: number = 5;

  private reconnectingInProgress = false;
  private authRenewed = false;

  constructor(protected localStorage: LocalStorageService,
              protected toastrService: ToastrService,
              protected authService: AuthService) {
  }

  protected createConnection(options: signalRConnectionOptions): signalR.HubConnection {
    
    let {       
      hubUri,
      logLevel,
      withAutoReconnect,
      withDefaultOnStateChangeActions,
      customReconnectDelays,
      reconnectOnConnectionDropPredicate
    } = options;

    let validatedLogLevel = this.validateLogLevel(logLevel);

    let builder = new signalR.HubConnectionBuilder()
      .withUrl(`${hubUri}`, {
        accessTokenFactory: async () => {
          // During reconnection attempt token might be already
          // expired, so it worth to renew it before proceeding.
          if(this.reconnectingInProgress && !this.authRenewed) {            
            this.authRenewed = true;
            const token = this.localStorage.getItem(AccessTokenItem);              
            await this.authService
              .refreshAccessToken({ accessToken: token})
              .toPromise();
          }

          return this.localStorage.getItem(AccessTokenItem)
        }
      })
      .configureLogging(validatedLogLevel);

    if (withAutoReconnect && customReconnectDelays) {
      builder.withAutomaticReconnect(customReconnectDelays);
    } else {
      builder.withAutomaticReconnect();
    }

    if (withDefaultOnStateChangeActions) {
      let connection = builder.build();

      connection.onreconnecting(() => {
        this.reconnectingInProgress = true;
        this.authRenewed = false;
        this.toastrService.warning(
          'Reconnecting to the server... Some notifications might be missing until done.', 
          'Notifications hub'
        );
      });
  
      connection.onreconnected(() => {
        this.reconnectingInProgress = false;
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
          this.totalReconnectAttempts = 0;
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
      console.error('Error while starting the connection: ' + err);
      if (this.totalReconnectAttempts < this.maxReconnectAttempts) {
        setTimeout(() => {
          this.totalReconnectAttempts++;
          this.start(connection);
        }, this.delayBeteweenReconnectOnStart);
      } else {
        console.error('Amount of reconnect attemps has reached max configured. No more recconects.')
      }
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

  private validateLogLevel(userSelectedLogLevel: signalR.LogLevel = null) {
    if (environment.production) {
      return signalR.LogLevel.Error;
    }

    return userSelectedLogLevel || signalR.LogLevel.Information;
  }
}


