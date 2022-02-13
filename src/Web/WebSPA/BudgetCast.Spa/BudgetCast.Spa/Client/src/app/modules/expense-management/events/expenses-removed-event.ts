import { ExpensesAppEvent } from "./expenses-event";

export class ExpensesRemovedAppEvent extends ExpensesAppEvent {
    removedBy: string;
    removedAt: Date;
}