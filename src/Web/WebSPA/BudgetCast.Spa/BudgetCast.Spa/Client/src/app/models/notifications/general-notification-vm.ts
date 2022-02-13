import { NotificationBase } from "./notification-base.vm";

export interface GeneralNotification extends NotificationBase {
    readonly data?: any;
}