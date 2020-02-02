import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { ReceiptItem } from '../models/receipt-item';
import { Receipt } from '../models/receipt';
import { MatTableDataSource } from '@angular/material';

@Component({
  selector: 'app-receipt-details',
  templateUrl: './receipt-details.component.html',
  styleUrls: ['./receipt-details.component.scss']
})
export class ReceiptDetailsComponent implements OnInit {
  
  receipt: Receipt;

  displayedColumns = ['id', 'title', 'price', 'quantity', 'description'];
  dataSource = new MatTableDataSource();

  constructor(private route: ActivatedRoute,              
              private location: Location) {    
  }

  ngOnInit() {
    this.receipt = this.route.snapshot.data.receipt;      
    this.dataSource.data = this.receipt.receiptItems;      
  }

  goBack() {
    this.location.back();
  }

}
