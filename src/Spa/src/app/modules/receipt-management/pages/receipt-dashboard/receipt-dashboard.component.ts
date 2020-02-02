import { Component, OnInit, ViewChild } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { BasicReceipt } from '../models/basic-receipt';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize, concatMap, tap } from 'rxjs/operators';
import { PageEvent, MatDialog } from '@angular/material';
import { CampaignService } from '../../services/campaign.service';
import { Subject, forkJoin } from 'rxjs';
import { TotalsPerCampaign } from '../models/totals-per-campaign';
import { CampaignTotalsComponent } from '../../components/campaign-totals/campaign-totals.component';

@Component({
  selector: 'app-receipt-dashboard',
  templateUrl: './receipt-dashboard.component.html',
  styleUrls: ['./receipt-dashboard.component.scss']
})
export class ReceiptDashboardComponent implements OnInit {

  private catSelectionSubj = new Subject<string>();
  catSelection$ = this.catSelectionSubj.asObservable();

  page = 1;
  pageSize = 10;
  pageSizeOptions = [5, 10, 15, 20, 50];

  campaign: string;
  campaignOptions: string[];

  total: number;
  items: BasicReceipt[] = [];
  totalsPerCampaign: TotalsPerCampaign;  

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

  constructor(private receiptService: RecipeService,
              private campaignService: CampaignService,
              private matDialog: MatDialog) { }

  ngOnInit() {
    const matcher = this.pageSizeMediaMatchers.find(m => m.key.matches);
    this.pageSize = matcher ? matcher.value : this.pageSizeOptions[1];

    this.spinner.show();

    this.campaignService.getAllCampaigns().pipe(
      tap(campaigns => {
        this.campaignOptions = campaigns ?
          campaigns.map(c => c.value) : campaigns;
        this.campaign = this.campaignOptions[0];
      }),

      concatMap(() => forkJoin([
        this.receiptService.getBasicReceipts(
          this.campaign, this.page, this.pageSize),
        this.receiptService.getTotalsPerCampaign(this.campaign)
      ])),

      finalize(() => this.spinner.hide())
    ).subscribe(([pageResult, totalsPerCampaign]) => {
        this.total = pageResult.total;
        this.items = pageResult.items;
        this.totalsPerCampaign = totalsPerCampaign;
      });
  }

  onMore(id: string) {
    console.log(id);
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
    this.receiptService.getBasicReceipts(
      this.campaign, this.page, this.pageSize).pipe(
      finalize(() => this.spinner.hide())
    )
    .subscribe(pageResult => {
      this.total = pageResult.total;
      this.items = pageResult.items;      
    });
  }

  initPage() {
    this.spinner.show();
    forkJoin([
      this.receiptService.getBasicReceipts(
        this.campaign, this.page, this.pageSize),
      this.receiptService.getTotalsPerCampaign(this.campaign)
    ]).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(([pageResult, totalsPerCampaign]) => {
      this.total = pageResult.total;
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
