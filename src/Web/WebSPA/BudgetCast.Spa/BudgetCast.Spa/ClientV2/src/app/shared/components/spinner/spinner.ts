import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.html',
  styleUrls: ['./spinner.scss'],
})
export class SpinnerComponent {
  public isVisible = signal(false);

  constructor() {}

  public show(): void {
    this.isVisible.set(true);
  }

  public hide(): void {
    this.isVisible.set(false);
  }
}
