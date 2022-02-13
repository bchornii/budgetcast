import { ExpensesAppEvent } from "./expenses-event";

export class ExpensesAddedAppEvent extends ExpensesAppEvent {
    id: string;
    total: number;
    addedBy: string;
    addedAt: Date;
}