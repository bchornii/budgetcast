import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account/account.service';
import { Subscription } from 'rxjs';
import { LoginCheck } from '../account/models/check-login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy {
  loginCheck: LoginCheck;
  authSubscription: Subscription;

  constructor(private accountService: AccountService,
              private router: Router) { }

  ngOnInit() {
    this.authSubscription = this.accountService.isUserAuthenticated$
    .subscribe(loginCheck => this.loginCheck = loginCheck);
  }

  ngOnDestroy() {
    this.authSubscription.unsubscribe();
  }

  logOut() {
    this.accountService.logout()
      .subscribe(_ => this.router.navigate(['/home']));
  }
}
