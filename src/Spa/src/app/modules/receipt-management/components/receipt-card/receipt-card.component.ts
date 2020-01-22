import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BasicReceipt } from '../../pages/models/basic-receipt';

@Component({
  selector: 'app-receipt-card',
  templateUrl: './receipt-card.component.html',
  styleUrls: ['./receipt-card.component.scss']
})
export class ReceiptCardComponent implements OnInit {

  @Input() receipt: BasicReceipt;
  @Output() onMore = new EventEmitter<void>();

  constructor() { }

  ngOnInit() {
  }

  more() {
    this.onMore.emit();
  }

}
