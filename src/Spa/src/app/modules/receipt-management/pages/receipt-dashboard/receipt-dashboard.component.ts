import { Component, OnInit, ViewChild } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { BasicReceipt } from '../models/basic-receipt';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { PageEvent } from '@angular/material';

@Component({
  selector: 'app-receipt-dashboard',
  templateUrl: './receipt-dashboard.component.html',
  styleUrls: ['./receipt-dashboard.component.scss']
})
export class ReceiptDashboardComponent implements OnInit {

  page = 1;
  pageSize = 10;
  pageSizeOptions = [5, 10, 15, 25, 50]
  campaign: string = 'January 2020';

  total: number;  
  items: BasicReceipt[] = [];

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: 520px)`);

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private receiptService: RecipeService) { }

  ngOnInit() {
    this.pageSize = this.mediaMatcher.matches
      ? this.pageSizeOptions[0] 
      : this.pageSizeOptions[1];
    this.initGrid();            
  }
  
  onMore(id: string) {
    console.log(id);
  }

  onPage($event: PageEvent) {
    this.page = $event.pageIndex + 1;
    this.pageSize = $event.pageSize;
    this.initGrid();
  }

  initGrid() {
    this.spinner.show();
    this.receiptService.getBasicReceipts(
      this.campaign, this.page, this.pageSize).pipe(
        finalize(() => this.spinner.hide())
      )
      .subscribe(pageResult => {
        this.total = pageResult.total;
        this.items = pageResult.items;
      });
  }
}
