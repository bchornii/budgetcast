import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <app-nav></app-nav>
    <app-login></app-login>
  `
})
export class AppComponent {
  title = 'budgetcast';
}
