import { ReceiptItem } from './receipt-item';

export interface Receipt {
    id: string;
    createdBy: string;
    createdAt: string;
    date: Date;
    description: string;
    tags: string[];
    receiptItems: ReceiptItem[];
}