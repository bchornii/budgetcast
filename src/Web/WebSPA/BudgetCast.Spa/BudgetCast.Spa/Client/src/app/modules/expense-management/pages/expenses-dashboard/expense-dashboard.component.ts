import { Component, OnInit, ViewChild } from '@angular/core';import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize, concatMap, tap } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { CampaignService } from '../../services/campaign.service';
import { Subject, forkJoin } from 'rxjs';
import { TotalsPerCampaignVm } from '../models/totals-per-campaign-vm';
import { CampaignTotalsComponent } from '../../components/campaign-totals/campaign-totals.component';
import { Router } from '@angular/router';
import { ExpenseVm } from '../models/expense-vm';
import { ExpensesService } from '../../services/expenses.service';

interface KeyValue<TKey, TValue> {
  key: TKey;
  value: TValue;
}

@Component({
  selector: 'app-expense-dashboard',
  templateUrl: './expense-dashboard.component.html',
  styleUrls: ['./expense-dashboard.component.scss']
})
export class ExpenseDashboardComponent implements OnInit {

  private catSelectionSubj = new Subject<string>();
  catSelection$ = this.catSelectionSubj.asObservable();

  page = 1;
  pageSize = 10;
  pageSizeOptions = [5, 10, 15, 20, 50];

  campaign: string;
  campaignOptions: string[];

  total: number;
  items: ExpenseVm[] = [];
  totalsPerCampaign: TotalsPerCampaignVm;

  pageSizeMediaMatchers: KeyValue<MediaQueryList, number>[] = [
    {
      key: matchMedia(`(max-width: 520px)`),
      value: this.pageSizeOptions[0]
    },
    {
      key: matchMedia(`(max-width: 1220px)`),
      value: this.pageSizeOptions[1]
    },
    {
      key: matchMedia(`(min-width: 1560px)`),
      value: this.pageSizeOptions[3]
    }
  ];

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private campaignService: CampaignService,
              private expensesService: ExpensesService,
              private matDialog: MatDialog,
              private router: Router) { }

  ngOnInit() {
    const matcher = this.pageSizeMediaMatchers.find(m => m.key.matches);
    this.pageSize = matcher ? matcher.value : this.pageSizeOptions[1];

    this.spinner.show();

    this.campaignService.getAllCampaigns().pipe(
      tap(campaigns => {
        this.campaignOptions = campaigns ?
          campaigns.map(c => c.name) : [];
        this.campaign = this.campaignOptions[0];
      }),

      concatMap(() => forkJoin([
        this.expensesService.getExpenses(
          this.campaign, this.page, this.pageSize),
        this.campaignService.getTotals(this.campaign)
      ])),

      finalize(() => this.spinner.hide())
    ).subscribe(([pageResult, totalsPerCampaign]) => {
        this.total = pageResult.totalCount;
        this.items = pageResult.items;
        this.totalsPerCampaign = totalsPerCampaign;
      });
  }

  onMore(receiptId: string) {
    this.spinner.show();
    this.router.navigate(['expenses/expense-details', receiptId])
      .finally(() => this.spinner.hide());
  }

  onPage($event: PageEvent) {
    this.initPager({
      page: $event.pageIndex + 1,
      pageSize: $event.pageSize
    });
    this.initGrid();
  }

  onCampaignValueChange() {
    this.initPager({
      page: 1,
      pageSize: this.pageSize
    });
    this.initPage();
  }

  initPager(pagerSettings: { page: number, pageSize: number }) {
    this.page = pagerSettings.page;
    this.pageSize = pagerSettings.pageSize;
  }

  initGrid() {
    this.spinner.show();
    this.expensesService.getExpenses(
      this.campaign, this.page, this.pageSize).pipe(
      finalize(() => this.spinner.hide())
    )
    .subscribe(pageResult => {
      this.total = pageResult.totalCount;
      this.items = pageResult.items;
    });
  }

  initPage() {
    this.spinner.show();
    forkJoin([
      this.expensesService.getExpenses(
        this.campaign, this.page, this.pageSize),
      this.campaignService.getTotals(this.campaign)
    ]).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(([pageResult, totalsPerCampaign]) => {
      this.total = pageResult.totalCount;
      this.items = pageResult.items;
      this.totalsPerCampaign = totalsPerCampaign;
    });
  }

  openTotals() {
    this.matDialog.open(CampaignTotalsComponent, {
      data: this.totalsPerCampaign.tagTotalPair
    });
  }
}
