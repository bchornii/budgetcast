import * as moment from 'moment';
import { Moment } from 'moment';

export class AddBasicReceipt {
    date: Moment;
    totalAmount: number;
    tags: string[];
    campaign: string;

    constructor() {
        this.date = moment();
        this.campaign = moment().format("MMMM YYYY");
        this.tags = ["Food"];
    }

    tagExists(value: string): boolean {
        return this.tags && this.tags.includes(value);
    }
}