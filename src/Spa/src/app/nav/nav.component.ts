import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account/account.service';
import { Observable } from 'rxjs';
import { UserIdentity } from '../account/models/check-login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
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
