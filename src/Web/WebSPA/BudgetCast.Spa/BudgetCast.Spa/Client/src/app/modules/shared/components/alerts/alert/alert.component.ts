import { Component, OnInit, Input } from '@angular/core';

enum AlertTypes {
  error = "error",
  info = "info"
}

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent {
  @Input() alertType = AlertTypes.error;

  get alertCss(): string[] {
    if (this.alertType == AlertTypes.info) {
      return ['app-alert--info'];
    }

    return ['app-alert--error'];
  }
}
