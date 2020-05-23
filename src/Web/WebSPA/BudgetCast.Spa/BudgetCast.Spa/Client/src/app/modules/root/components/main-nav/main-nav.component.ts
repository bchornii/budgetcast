import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs/internal/Subscription';
import { filter } from 'rxjs/operators';
import { ROUTE_NAV_ITEM_MAPPING } from '../../routes-nav-map';

@Component({
  selector: 'app-main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent implements OnInit, OnDestroy {

  private routerSubscription: Subscription;

  openedSubmenu: string = null;

  constructor(private authService: AuthService,
              private router: Router) {

    this.routerSubscription = this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe((evt: NavigationEnd) => {

      const routeNavItem = ROUTE_NAV_ITEM_MAPPING
        .find(item => evt.urlAfterRedirects.includes(item.route));

      if (routeNavItem) {
        this.openSubMenu(routeNavItem.navitem, true);
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();
  }

  openSubMenu(subMenuName: string, isAutoNav = false): void {
    if (this.openedSubmenu === subMenuName && !isAutoNav) {
      this.openedSubmenu = null;
    } else {
      this.openedSubmenu = subMenuName;
    }
  }

  logOut() {
    this.authService.logout().subscribe();
    this.router.navigate(['/welcome']);
  }
}
