import { CampaignService } from './../../services/campaign.service';
import { NgModel } from '@angular/forms';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { finalize } from 'rxjs/operators';

import { AddBasicReceipt } from '../models/add-receipt';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ExpensesService } from '../../services/expenses.service';


@Component({
  selector: 'app-add-receipt',
  templateUrl: './add-receipt.component.html'
})
export class AddReceiptComponent implements OnInit {

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;
  @ViewChild('totalAmount', { static: true, read: NgModel }) totAmount: NgModel;

  tagsLoading = false;
  tagOptions = [];

  campaignsLoading = false;
  campaignOptions = [];

  addBasicReceipt = new AddBasicReceipt();

  constructor(private recipeService: RecipeService,
              private campaignService: CampaignService,
              private expensesService: ExpensesService,
              private router: Router,
              private toastr: ToastrService) { }

  ngOnInit() {
  }

  onTagModelChange(data) {
    if (!this.addBasicReceipt.tagExists(data)) {
      this.tagsLoading = true;
      this.expensesService.searchTags(data).pipe(
        finalize(() => this.tagsLoading = false)
      )
        .subscribe(r => this.tagOptions = r);
    }
  }

  onCampaignModelChange(data) {
    this.campaignsLoading = true;
    this.campaignService.getCampaigns(data).pipe(
      finalize(() => this.campaignsLoading = false)
    )
      .subscribe(r => this.campaignOptions = r);
  }

  addReceipt() {
    this.spinner.show();
    this.recipeService.addBasicReceipt(this.addBasicReceipt).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(_ => {
      this.toastr.success('Receipt added.');
      this.router.navigate(['/receipts/dashboard']);
    });
  }
}
