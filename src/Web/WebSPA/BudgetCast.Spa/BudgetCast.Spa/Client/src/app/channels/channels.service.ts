import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { NotificationBase } from "../models/notifications/notification-base.vm";
import { ExpensesAddedAppEvent } from "../modules/expense-management/events/expenses-added-event";
import { ExpensesRemovedAppEvent } from "../modules/expense-management/events/expenses-removed-event";
import { ChannelTypes } from "./channel-types";
import { AppEvent } from "./app-event";
import { AppEventsMapper } from "./app-events-mapper.service";

@Injectable({
    providedIn: 'root'
  })
export class ChannelsService {

    private appEventToChannelMap = new Map<string, ChannelTypes>();

    private sources = new Map<ChannelTypes, Subject<AppEvent>>();
    public channels$ = new Map<ChannelTypes, Observable<AppEvent>>();

    constructor(private appEventsMapper: AppEventsMapper) {

        this.mapAppEventToChannels();

        for(let channelType of Object.values(ChannelTypes)) {
            const channelSource = new Subject<AppEvent>();
            this.sources.set(channelType, channelSource);
            this.channels$.set(channelType, channelSource.asObservable());
        }
    }

    write(notification: NotificationBase) {
        const appEvent = this.appEventsMapper.mapFrom(notification);
        const channelType = this.appEventToChannelMap.get(appEvent.eventName);
        const channelSource = this.sources.get(channelType);

        channelSource.next(appEvent);
    }

    private mapAppEventToChannels(): void {
        this.mapExpensesAppEventsToChannels();
    }

    private mapExpensesAppEventsToChannels(): void {
        this.appEventToChannelMap.set(ExpensesAddedAppEvent.name, ChannelTypes.Expenses);
        this.appEventToChannelMap.set(ExpensesRemovedAppEvent.name, ChannelTypes.Expenses);
    }
}