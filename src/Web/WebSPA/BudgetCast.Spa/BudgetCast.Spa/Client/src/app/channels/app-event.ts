/**
 * Represents an application event.
 * @description
 * Used internally by the application (WebUI part) to handle varios types
 * of events happening in the system. 
 */
export class AppEvent {
    readonly eventName: string;
    readonly message?: string;
}