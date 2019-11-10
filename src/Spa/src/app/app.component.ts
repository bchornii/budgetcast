import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterEvent } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  template: `
    <app-nav *ngIf="diplayNavBar"></app-nav>
    <router-outlet></router-outlet>
  `
})
export class AppComponent implements OnInit {
  diplayNavBar = true;

  constructor(private router: Router) {}

  ngOnInit() {

    this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe((evt: RouterEvent) => {
      this.diplayNavBar = !evt.url.includes('login');
    });
  }
}
