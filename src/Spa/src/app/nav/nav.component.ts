import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account/account.service';
import { Subscription } from 'rxjs';
import { UserIdentity } from '../account/models/check-login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy {
  userIdentity: UserIdentity;
  authSubscription: Subscription;

  constructor(private accountService: AccountService,
              private router: Router) { }

  ngOnInit() {
    this.authSubscription = this.accountService.userIdentity$
      .subscribe(userIdentity => this.userIdentity = userIdentity);
  }

  ngOnDestroy() {
    this.authSubscription.unsubscribe();
  }

  logOut() {
    this.accountService.logout()
      .subscribe(_ => this.router.navigate(['/home']));
  }
}
