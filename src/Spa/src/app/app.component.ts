import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterEvent } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  template: `
    <div [ngClass]="getContainerCss()">
      <app-nav *ngIf="diplayNavBar"></app-nav>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AppComponent implements OnInit {
  diplayNavBar = false;

  constructor(private router: Router) {}

  ngOnInit() {

    this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe((evt: RouterEvent) => {
      const urlParts = evt.url.includes('?')
        ? evt.url.slice(0, evt.url.indexOf('?')).split('/')
        : evt.url.split('/');
      this.diplayNavBar = !this.getNonHeaderRoutes()
        .includes(urlParts[urlParts.length - 1]);
    });
  }

  getContainerCss() {
    if (this.diplayNavBar) {
      return ['container'];
    }

    return ['container-fluid'];
  }

  private getNonHeaderRoutes() {
    return ['login', 'register', 'forgot-password', 'reset-password', 'home'];
  }
}
