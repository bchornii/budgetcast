import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { RecipeService } from '../services/receipt.service';
import { Receipt } from '../pages/models/receipt';

@Injectable({
    providedIn: 'root'
})
export class ReceiptDetailsResolver implements Resolve<Observable<Receipt>> {

    constructor(private receiptService: RecipeService) {}

    resolve(route: ActivatedRouteSnapshot) : Observable<Receipt> {
        return this.receiptService.getReceiptDetails(route.params['id']);
    }
}