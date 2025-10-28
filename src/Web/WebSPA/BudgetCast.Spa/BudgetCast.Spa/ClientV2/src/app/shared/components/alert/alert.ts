import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

enum AlertTypes {
  error = 'error',
  info = 'info',
}

@Component({
  selector: 'app-alert',
  templateUrl: './alert.html',
  styleUrls: ['./alert.scss'],
  imports: [MatIcon, NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AlertComponent {
  alertType = input<string>(AlertTypes.error);

  get alertCss(): string[] {
    if (this.alertType() == AlertTypes.info) {
      return ['app-alert--info'];
    }

    return ['app-alert--error'];
  }
}
