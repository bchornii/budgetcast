import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExpenseVm } from '../../pages/models/expense-vm';

@Component({
  selector: 'app-receipt-card',
  templateUrl: './receipt-card.component.html',
  styleUrls: ['./receipt-card.component.scss']
})
export class ReceiptCardComponent implements OnInit {

  @Input() expense: ExpenseVm;
  @Output() onMore = new EventEmitter<void>();

  constructor() { }

  ngOnInit() {
  }

  more() {
    this.onMore.emit();
  }

}
