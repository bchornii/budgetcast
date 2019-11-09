import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account/account.service';
import { Subscription } from 'rxjs';
import { LoginCheck } from '../account/models/check-login.model';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy {
  loginCheck: LoginCheck;
  subscription: Subscription;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.subscription = this.accountService.isUserAuthenticated$
    .subscribe(loginCheck => this.loginCheck = loginCheck);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
