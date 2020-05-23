import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent implements OnInit {
  public isVisible = false;

  constructor() { }

  ngOnInit() { }

  public show(): void {
    this.isVisible = true;
  }

  public hide(): void {
    this.isVisible = false;
  }
}
