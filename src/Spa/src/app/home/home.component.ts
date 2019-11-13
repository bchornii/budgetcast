import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account/account.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  private authSubscription: Subscription;
  private isAuthenticated = false;

  constructor(private accountService: AccountService,
              private router: Router) { }

  ngOnInit() {
    this.authSubscription = this.accountService.userIdentity$
      .subscribe(r => this.isAuthenticated = r.isAuthenticated);
  }

  ngOnDestroy() {
    this.authSubscription.unsubscribe();
  }

  getStarted() {
    this.isAuthenticated
      ? this.router.navigate(['/home'])
      : this.router.navigate(['/account/login']);
  }
}
