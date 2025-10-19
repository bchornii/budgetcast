import { format } from 'date-fns';

export class AddExpenseDto {
    addedAt: Date;
    totalAmount: number;
    tags: string[];
    campaignName: string;
    description: string;

    constructor() {
        this.addedAt = new Date();
        this.campaignName = format(new Date(), 'MMMM yyyy');
        this.tags = [] as string[];
    }

    tagExists(value: string): boolean {
        return this.tags && this.tags.includes(value);
    }
}