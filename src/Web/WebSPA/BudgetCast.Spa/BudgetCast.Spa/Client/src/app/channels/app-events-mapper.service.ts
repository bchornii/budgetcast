import { Injectable } from "@angular/core";
import { GeneralNotification } from "../models/notifications/general-notification-vm";
import { NotificationMessageTypes } from "../models/notifications/notification-message-types";
import { ExpensesAddedAppEvent } from "../modules/expense-management/events/expenses-added-event";
import { ExpensesRemovedAppEvent } from "../modules/expense-management/events/expenses-removed-event";
import { AppEvent } from "./app-event";

@Injectable({
    providedIn: 'root'
  })
export class AppEventsMapper {
    
    private notificationsMap = new Map<string, ((notification: GeneralNotification) => AppEvent)>()

    constructor() {
        this.initializeMaps();
    }

    mapFrom(notification: GeneralNotification): AppEvent {
        let mapToAppEvent = this.notificationsMap.get(notification.messageType);
        return mapToAppEvent(notification);
    }

    private initializeMaps(): void {
        this.notificationsMap.set
        (
            NotificationMessageTypes.ExpensesAdded, 
            this.toExpensesAddedAppEvent
        );

        this.notificationsMap.set
        (
            NotificationMessageTypes.ExprensesRemoved,
            this.toExpensesRemovedAppEvent
        );
    }

    private toExpensesAddedAppEvent(notification: GeneralNotification): ExpensesAddedAppEvent {
        return {
            eventName: ExpensesAddedAppEvent.name,
            message: notification.message,
            id: notification.data.id,
            total: notification.data.total,
            addedBy: notification.data.addedBy,
            addedAt: notification.data.addedAt
        };
    }

    private toExpensesRemovedAppEvent(notification: GeneralNotification): ExpensesRemovedAppEvent {
        return {
            eventName: ExpensesAddedAppEvent.name,
            message: notification.message,
            removedAt: notification.data.removedAt,
            removedBy: notification.data.removedBy
        };
    }
}