import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { NotificationBase } from "../models/notifications/notification-base.vm";
import { ExpensesAddedAppEvent } from "../modules/expense-management/events/expenses-added-event";
import { ExpensesRemovedAppEvent } from "../modules/expense-management/events/expenses-removed-event";
import { ChannelTypes } from "./channel-types";
import { AppEvent } from "./app-event";
import { AppEventsMapper } from "./app-events-mapper.service";

/**
 * Represents an internal to the application communication bus based on a set of channels
 * which act as an abstraction layer over transport mechanism for sending and receiving 
 * application events. 
 * 
 * Each channel itself reprensents pub/sub communication style, i.e. you may send an event
 * which may be potentially received by any number of subscribers.
 * 
 * @description
 * In a current implementation channel is represented by Subject<AppEvent> type on a sending end
 * and Observable<AppEvent> on a receiving end.
 */
@Injectable({
    providedIn: 'root'
  })
export class ChannelsService {

    /**
     * Maps event names to ChannelTypes @enum values.
     * @description 
     * For example, events that belong to specific application area,
     * such as expenses will be mapped to the same channel type which coresponds to
     * the same application area as well.
     */
    private appEventToChannelMap = new Map<string, ChannelTypes>();

    /**
     * Maps values of ChannelTypes @enum to subjects of @class AppEvent.
     * @description
     * Each channel type will be mapped to an appropriate subject of type 
     * derived from AppEvent. The subject will be used to send events.
     */
    private sources = new Map<ChannelTypes, Subject<AppEvent>>();

    /**
     * Maps values of ChannelTypes @enum to observables of @class AppEvent.
     * @description
     * Each channel type will be mapped to an appropriate observable of type 
     * derived from AppEvent. The observable will be used to receive (aka. subscribe) 
     * to events.
     */
    public channels$ = new Map<ChannelTypes, Observable<AppEvent>>();

    constructor(private appEventsMapper: AppEventsMapper) {

        this.mapAppEventToChannels();

        for(let channelType of Object.values(ChannelTypes)) {
            const channelSource = new Subject<AppEvent>();
            this.sources.set(channelType, channelSource);
            this.channels$.set(channelType, channelSource.asObservable());
        }
    }

    /**
     * Writes notification to an appropriate channel.
     * @param notification - instance of type derived from NotificationBase
     * @description
     * Under the hood each notification type is mapped to an appropriate
     * event
     */
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