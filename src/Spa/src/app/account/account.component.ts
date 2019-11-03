import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AccountService } from './account.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit, OnDestroy {
  isUserAuthenticated = false;
  subscription: Subscription;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.subscription = this.accountService.isUserAuthenticated$
      .subscribe(isAuthenticated => {
        this.isUserAuthenticated = isAuthenticated;
      });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  login() {
    this.accountService.login();
  }

  logout() {
    this.accountService.logout()
      .subscribe();
  }

  simulateFailedCall() {
    this.accountService.checkUserAuthenticationStatus()
      .subscribe();
  }

  check() {
    this.accountService.check()
      .subscribe();
  }
}
