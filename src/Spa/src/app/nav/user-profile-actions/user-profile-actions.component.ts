import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { AccountService } from 'src/app/account/account.service';
import { Router } from '@angular/router';
import { UserIdentity } from 'src/app/account/models/check-login.model';
import { Subscription, Observable } from 'rxjs';

@Component({
  selector: 'app-user-profile-actions',
  templateUrl: './user-profile-actions.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserProfileActionsComponent implements OnInit {
  userIdentity$: Observable<UserIdentity>;

  constructor(private accountService: AccountService,
    private router: Router) { }

  ngOnInit() {
    this.userIdentity$ = this.accountService.userIdentity$;      
  }

  logOut() {
    this.accountService.logout()
      .subscribe(_ => this.router.navigate(['/home']));
  }
}
