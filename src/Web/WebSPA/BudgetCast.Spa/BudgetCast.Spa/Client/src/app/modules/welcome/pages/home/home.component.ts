import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  isAuthenticated = false;

  private authSubscription: Subscription;

  constructor(private authService: AuthService,
              private router: Router) { }

  ngOnInit() {
    this.authSubscription = this.authService.userIdentity$
      .subscribe(r => this.isAuthenticated = r.isAuthenticated);
  }

  ngOnDestroy() {
    this.authSubscription.unsubscribe();
  }

  getStarted() {
    this.isAuthenticated
      ? this.router.navigate(['/receipts/dashboard'])
      : this.router.navigate(['/welcome/login']);
  }

  logOut() {
    this.authService.logout().subscribe();
  }
}
