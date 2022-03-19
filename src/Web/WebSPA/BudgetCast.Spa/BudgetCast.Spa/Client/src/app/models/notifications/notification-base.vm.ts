import { NotificationType } from "./notification-type-vm";

/**
 * Represents notification received from an external resource.
 * @description
 * The most basic type of notification is the one received via web sockets
 * channel.
 */
export interface NotificationBase {
    readonly type: NotificationType;
    readonly message?: string;
    readonly messageType: string;    
}