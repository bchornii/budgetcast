import { NotificationType } from "./notification-type-vm";

export interface NotificationBase {
    readonly type: NotificationType;
    readonly message?: string;
    readonly messageType: string;    
}