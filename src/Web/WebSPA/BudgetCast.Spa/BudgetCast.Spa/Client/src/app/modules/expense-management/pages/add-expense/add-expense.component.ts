import { CampaignService } from '../../services/campaign.service';
import { NgModel } from '@angular/forms';
import { Component, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ExpensesService } from '../../services/expenses.service';
import { AddExpenseDto } from '../models/add-expense-dto';


@Component({
  selector: 'app-add-expense',
  templateUrl: './add-expense.component.html'
})
export class AddExpenseComponent implements OnInit {

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;
  @ViewChild('totalAmount', { static: true, read: NgModel }) totAmount: NgModel;

  tagsLoading = false;
  tagOptions = [];

  campaignsLoading = false;
  campaignOptions = [];

  expenseDto = new AddExpenseDto();

  constructor(private campaignService: CampaignService,
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
      this.toastr.success('Expense sent for processing.');
      this.router.navigate(['/expenses/dashboard']);
    });
  }
}
