import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatTableDataSource } from '@angular/material/table';
import { ExpenseDetailsVm } from '../models/expense-details-vm';

@Component({
  selector: 'app-expense-details',
  templateUrl: './expense-details.component.html',
  styleUrls: ['./expense-details.component.scss']
})
export class ExpenseDetailsComponent implements OnInit {
  
  expense: ExpenseDetailsVm;

  displayedColumns = ['id', 'title', 'price', 'quantity', 'note'];
  dataSource = new MatTableDataSource();

  constructor(private route: ActivatedRoute,              
              private location: Location) {    
  }

  ngOnInit() {
    this.expense = this.route.snapshot.data.expense;      
    this.dataSource.data = this.expense.expenseItems;      
  }

  goBack() {
    this.location.back();
  }

}
