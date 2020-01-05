import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';
import { Subscription } from 'rxjs';
import { ROUTE_NAV_ITEM_MAPPING } from './routes-nav-map';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent implements OnInit, OnDestroy {

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private routerSubscription: Subscription;

  openedSubmenu: string = null;
  userIdentity$ = this.authService.userIdentity$;

  @ViewChild('snav', { static: false }) snav: MatSidenav;

  constructor(private authService: AuthService,
              private router: Router) {

    this.routerSubscription = this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe((evt: NavigationEnd) => {
      if (this.isScreenSmall() && this.snav) {
        this.snav.close();
      }

      const routeNavItem = ROUTE_NAV_ITEM_MAPPING
        .find(item => evt.urlAfterRedirects.includes(item.route));

      if (routeNavItem) {
        console.log('Before:', this.openedSubmenu);
        this.openSubMenu(routeNavItem.navitem, true);
        console.log('After:', this.openedSubmenu);
      }
    });
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }

  openSubMenu(subMenuName: string, isAutoNav = false): void {
    if (this.openedSubmenu === subMenuName && !isAutoNav) {
      this.openedSubmenu = null;
    } else {
      this.openedSubmenu = subMenuName;
    }
  }

  isSubMenuOpened(): boolean {
    return this.openSubMenu != null;
  }

  logOut() {
    this.authService.logout().subscribe();
    this.router.navigate(['/welcome']);
  }
}
