import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { take } from 'rxjs';
import { Auth } from './core/auth/services/auth';
import { EnvDemo } from './shared/components/env-demo/env-demo';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, EnvDemo],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly auth = inject(Auth);

  protected readonly title = signal('bg-portal');

  onAuthTestClick(): void {
    this.auth
      .checkUserAuthenticationStatus()
      .pipe(take(1))
      .subscribe({
        next: (identity) => console.log('Auth status:', identity),
        error: (err) => console.error('Auth status check failed:', err),
      });
  }
}
