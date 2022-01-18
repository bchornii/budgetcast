import { ExpenseItemDetailsVm } from './expense-item-details-vm';

export interface ExpenseDetailsVm {
    id: number;
    campaignId: number;
    addedBy: string;
    createdOn: string;
    addedAt: Date;
    description: string;
    tags: string[];
    expenseItems: ExpenseItemDetailsVm[];
}