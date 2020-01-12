import { NgModel } from '@angular/forms';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { finalize, delay } from 'rxjs/operators';

import { AddBasicReceipt } from '../models/add-receipt';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { of } from 'rxjs';


@Component({
  selector: 'app-add-receipt',
  templateUrl: './add-receipt.component.html'
})
export class AddReceiptComponent implements OnInit {
  
  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;
  @ViewChild('totalAmount', {static: true, read: NgModel}) totAmount: NgModel;
  
  tagsLoading = false;
  tagOptions = [];

  campaignsLoading = false;
  campaignOptions = [];

  addBasicReceipt = new AddBasicReceipt();

  constructor(private recipeService: RecipeService) { }

  ngOnInit() { 
  }

  onTagModelChange(data) {
    console.log(this.addBasicReceipt.tags);
    if (!this.addBasicReceipt.tagExists(data)) {
      this.tagsLoading = true;
      this.recipeService.getCategories(data).pipe(
        finalize(() => this.tagsLoading = false)
      )
      .subscribe(r => this.tagOptions = r);
    }
  }

  onCampaignModelChange(data) {    
    this.campaignsLoading = true;
    this.recipeService.getCategories(data).pipe(
      finalize(() => this.campaignsLoading = false)
    )
    .subscribe(r => this.campaignOptions = r);
  }

  addReceipt() {      
    this.spinner.show();
    of(null).pipe(
      delay(1000),
      finalize(() => this.spinner.hide())
    ).subscribe(_ => {
      console.log(JSON.stringify(this.addBasicReceipt));
    });    
  }
}
