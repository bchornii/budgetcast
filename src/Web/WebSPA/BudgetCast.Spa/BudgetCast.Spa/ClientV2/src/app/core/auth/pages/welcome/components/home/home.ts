import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { Auth } from '../../../../services/auth';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
  imports: [CommonModule, RouterModule],
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  isAuthenticated = signal(false);

  private authSubscription: Subscription | undefined;

  ngOnInit() {
    this.authSubscription = this.authService.userIdentity$.subscribe((r) =>
      this.isAuthenticated.set(r.isAuthenticated),
    );
  }

  ngOnDestroy() {
    this.authSubscription?.unsubscribe();
  }

  getStarted() {
    if (this.isAuthenticated()) {
      this.router.navigate(['/expenses/dashboard']);
    } else {
      this.router.navigate(['/welcome/login']);
    }
  }

  logOut() {
    this.authService.logout().subscribe();
  }
}
