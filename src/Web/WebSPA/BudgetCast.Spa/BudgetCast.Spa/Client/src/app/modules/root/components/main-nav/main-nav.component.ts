import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs/internal/Subscription';
import { filter } from 'rxjs/operators';
import { ROUTE_NAV_ITEM_MAPPING } from '../../routes-nav-map';
import { ArrayExtensions } from 'src/app/util/extensions/array-extensions';

@Component({
  selector: 'app-main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent implements OnInit, OnDestroy {

  private routerSubscription: Subscription;
  
  openedSubMenus: string[] = [];

  constructor(private authService: AuthService,
              private router: Router) {

    this.routerSubscription = this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe((evt: NavigationEnd) => {

      const routeNavItem = ROUTE_NAV_ITEM_MAPPING
        .find(item => evt.urlAfterRedirects.includes(item.route));

      if (routeNavItem) {
        this.flipSubMenu(routeNavItem.navitem, false);
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.routerSubscription.unsubscribe();
  }

  isExpanded(subMenuName: string): boolean {
    return !!this.openedSubMenus.find(x => x == subMenuName);
  }

  indexOfElement(subMenuName: string): number {
    return this.openedSubMenus.indexOf(subMenuName);
  }

  flipSubMenu(subMenuName: string, isNotRoutingNavigation = true): void {
    const expandedElementIdx = this.indexOfElement(subMenuName);
    const isExpanded = expandedElementIdx != -1;
    const isNotExpanded = expandedElementIdx == -1;

    if(isExpanded && isNotRoutingNavigation) {
      this.openedSubMenus.removeAt(expandedElementIdx);
    } 

    if(isNotExpanded) {
      this.openedSubMenus.push(subMenuName);
    }
  }

  logOut() {
    this.authService.logout().subscribe();
    this.router.navigate(['/welcome']);
  }
}