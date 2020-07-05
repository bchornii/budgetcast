import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { KeyValuePair } from 'src/app/util/util';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-campaign-totals',
  templateUrl: './campaign-totals.component.html',
  styleUrls: ['./campaign-totals.component.scss']
})
export class CampaignTotalsComponent implements OnInit {
  
  displayedColumns = ['key', 'value'];
  dataSource = new MatTableDataSource(this.data);

  @ViewChild(MatSort, {static: true}) sort: MatSort;
  
  constructor(@Inject(MAT_DIALOG_DATA) 
              public data: KeyValuePair[]) { }

  ngOnInit() {
    this.dataSource.sort = this.sort;    
  }

}
