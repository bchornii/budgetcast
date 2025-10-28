import { Component } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.html',
  styleUrls: ['./spinner.scss'],
})
export class SpinnerComponent {
  public isVisible = false;

  constructor() {}

  public show(): void {
    this.isVisible = true;
  }

  public hide(): void {
    this.isVisible = false;
  }
}
