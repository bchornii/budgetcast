import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatSidenav } from '@angular/material/sidenav';
import { Subscription } from 'rxjs';
import { SMALL_WIDTH_BREAKPOINT } from 'src/app/util/constants/response-status';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent implements OnInit, OnDestroy {

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private routerSubscription: Subscription;

  userIdentity$ = this.authService.userIdentity$;

  @ViewChild('snav') snav: MatSidenav;

  constructor(private authService: AuthService,
              private router: Router) {

    this.routerSubscription = this.router.events.pipe(
      filter(evt => evt instanceof NavigationEnd)
    ).subscribe(_ => {
      if (this.isScreenSmall() && this.snav) {
        this.snav.close();
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
}
