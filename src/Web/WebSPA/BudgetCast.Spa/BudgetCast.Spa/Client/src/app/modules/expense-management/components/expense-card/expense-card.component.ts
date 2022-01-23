import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExpenseVm } from '../../pages/models/expense-vm';

@Component({
  selector: 'app-expense-card',
  templateUrl: './expense-card.component.html',
  styleUrls: ['./expense-card.component.scss']
})
export class ExpenseCardComponent implements OnInit {

  @Input() expense: ExpenseVm;
  @Output() onMore = new EventEmitter<void>();

  constructor() { }

  ngOnInit() {
  }

  more() {
    this.onMore.emit();
  }

}
