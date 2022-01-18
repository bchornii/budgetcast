export interface ExpenseVm {
    id: number;
    createdBy: string;
    addedAt: Date;
    description: string;
    totalItems?: number;
    totalAmount?: number;
    tags: string[];
}