import { CampaignService } from './../../services/campaign.service';
import { NgModel } from '@angular/forms';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { finalize } from 'rxjs/operators';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ExpensesService } from '../../services/expenses.service';
import { AddExpenseDto } from '../models/add-expense-dto';


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

  expenseDto = new AddExpenseDto();

  constructor(private recipeService: RecipeService,
              private campaignService: CampaignService,
              private expensesService: ExpensesService,
              private router: Router,
              private toastr: ToastrService) { }

  ngOnInit() {
  }

  onTagModelChange(data) {
    if (!this.expenseDto.tagExists(data)) {
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

  addExpense() {
    this.spinner.show();
    this.expensesService.addExpense(this.expenseDto).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(_ => {
      this.toastr.success('Expense added.');
      this.router.navigate(['/receipts/dashboard']);
    });
  }
}
