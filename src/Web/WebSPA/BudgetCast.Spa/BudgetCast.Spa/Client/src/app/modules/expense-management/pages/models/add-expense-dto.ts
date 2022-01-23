import * as moment from 'moment';
import { Moment } from 'moment';

export class AddExpenseDto {
    addedAt: Moment;
    totalAmount: number;
    tags: string[];
    campaignName: string;
    description: string;

    constructor() {
        this.addedAt = moment();
        this.campaignName = moment().format("MMMM YYYY");
        this.tags = [] as string[];
    }

    tagExists(value: string): boolean {
        return this.tags && this.tags.includes(value);
    }
}