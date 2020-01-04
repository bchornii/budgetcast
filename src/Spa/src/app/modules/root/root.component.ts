import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router, NavigationEnd, NavigationStart, RouterEvent } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent implements OnInit {

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);

  openedSubmenu: string = null;
  userIdentity$ = this.authService.userIdentity$;

  @ViewChild('snav', {static: false}) snav: MatSidenav;

  constructor(private authService: AuthService,
              private router: Router) { }

  ngOnInit() {
    this.router.events.pipe(
      filter(evt => evt instanceof NavigationStart)
    ).subscribe(_ => {
      if (this.isScreenSmall()) {
        this.snav.close();
      }
    });
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }

  openSubMenu(subMenuName: string): void {
    if (this.openedSubmenu === subMenuName) {
      this.openedSubmenu = null;
    } else {
      this.openedSubmenu = subMenuName;
    }
  }

  isSubMenuOpened(): boolean {
    return this.openSubMenu != null;
  }
}
