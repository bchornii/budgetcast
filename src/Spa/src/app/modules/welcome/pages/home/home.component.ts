import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  isAuthenticated = false;

  private authSubscription: Subscription;

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
      ? this.router.navigate(['/receipt/receipt-dashboard'])
      : this.router.navigate(['/welcome/login']);
  }

  logOut() {
    this.accountService.logout().subscribe();
  }
}
