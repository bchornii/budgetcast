export interface BasicReceipt {
    id: string;
    createdBy: string;
    date: Date;
    description: string;
    totalItems?: number;
    totalAmount: number;
    tags: string[];
}