import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

const SMALL_WIDTH_BREAKPOINT = 720;

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent implements OnInit {

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);

  get userIdentity$() {
    return this.authService.userIdentity$;
  }

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }

  closeSideNav(snav: any) {
    if (this.isScreenSmall()) {
      snav.close();
    }
  }
}
