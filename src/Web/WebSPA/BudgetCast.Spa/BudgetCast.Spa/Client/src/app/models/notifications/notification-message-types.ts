/**
 * Notification types which might be sent by the notification service.
 */
export enum NotificationMessageTypes {
    Unknown = "Unknown",

    // expenses
    ExpensesAdded = "ExpensesAdded",
    ExprensesRemoved = "ExpensesRemoved",

    // campaigns
    CampaignAdded = "CampaignAdded",
    CampaignJoined = "CampaignJoined"
}