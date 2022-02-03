import { NotificationType } from "./notification-type-vm";

export interface BasicNotification {
 messageType: string;
 message?: string;
 type: NotificationType;
}