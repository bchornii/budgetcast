import * as signalR from "@microsoft/signalr";


export class signalRConnectionOptions {
  private _hubUri: string;
  private _logLevel: signalR.LogLevel;
  private _withAutoReconnect: boolean;
  private _withAuth: boolean;
  private _withDefaultOnStateChangeActions: boolean;
  private _customReconnectDelays: number[];

  private _reconnectOnConnectionDropPredicate: () => boolean;

  public get hubUri() {
    return this._hubUri;
  }

  public get logLevel() {
    return this._logLevel;
  }

  public get withAuth() {
    return this._withAuth;
  }

  public get withAutoReconnect() {
    return this._withAutoReconnect;
  }

  public get withDefaultOnStateChangeActions() {
    return this._withDefaultOnStateChangeActions;
  }

  public get customReconnectDelays() {
    return this._customReconnectDelays;
  }

  public get reconnectOnConnectionDropPredicate() {
    return this._reconnectOnConnectionDropPredicate;
  }

  constructor() {
    this._logLevel = signalR.LogLevel.Information;
    this._withAuth = true;
    this._withAutoReconnect = true;
    this._withDefaultOnStateChangeActions = true;
    this._reconnectOnConnectionDropPredicate = () => false;
  }

  withUri(hubUri: string): signalRConnectionOptions {
    this._hubUri = hubUri;
    return this;
  }

  withReconnectOnConnectionDropPredicate(predicate: () => boolean): signalRConnectionOptions {
    this._reconnectOnConnectionDropPredicate = predicate;
    return this;
  }
}
