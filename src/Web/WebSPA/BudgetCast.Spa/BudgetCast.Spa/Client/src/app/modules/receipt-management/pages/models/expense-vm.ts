export interface ExpenseVm {
    id: string;
    createdBy: string;
    addedAt: Date;
    description: string;
    totalItems?: number;
    totalAmount?: number;
    tags: string[];
}