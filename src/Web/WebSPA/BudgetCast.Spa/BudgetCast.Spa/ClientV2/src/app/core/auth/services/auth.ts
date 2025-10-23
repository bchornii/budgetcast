import { HttpClient } from '@angular/common/http';
import { DOCUMENT, inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { BaseService } from '../../services/base-service';
import { Configuration } from '../../services/configuration/configuration';
import { ForgotPasswordDto } from '../models/forgot-password-dto';
import { ResetPasswordDto } from '../models/reset-password-dto';
import { UserIdentity } from '../models/user-identity-vm';
import { UserRegistrationDto } from '../models/user-registration-dto';

@Injectable({
  providedIn: 'root',
})
export class Auth extends BaseService {
  private document = inject(DOCUMENT);
  private httpClient = inject(HttpClient);
  private configuration = inject(Configuration);

  private invalidUserIdentity = new UserIdentity();
  private userIdentitySubject = new BehaviorSubject<UserIdentity>(this.invalidUserIdentity);
  userIdentity$ = this.userIdentitySubject.asObservable();

  get userIdentity(): UserIdentity {
    return this.userIdentitySubject.value;
  }
  get isAuthenticated(): boolean {
    return this.userIdentity.isAuthenticated;
  }

  checkUserAuthenticationStatus(): Observable<UserIdentity> {
    this.log(
      'info',
      `Making request to ${this.configuration.endpoints.identity.account.isAuthenticated}`,
    );

    const url = `${this.configuration.endpoints.identity.account.isAuthenticated}`;
    const request = this.httpClient.get<UserIdentity>(url).pipe(
      tap((r) => {
        this.log('info', `User authentication status: ${JSON.stringify(r)}`);
        this.userIdentitySubject.next(r);
      }),
      this.retryRequest(2, 500),
    );

    return this.executeRequest(request, 'Checking user authentication status', url);
  }

  googleLogin(): void {
    this.document.location.href = `${this.configuration.endpoints.identity.signIn.google}`;
  }

  facebookLogin(): void {
    this.document.location.href = `${this.configuration.endpoints.identity.signIn.facebook}`;
  }

  logout() {
    const url = `${this.configuration.endpoints.identity.signOut.all}`;

    this.log('info', `Making request to ${url} to log out user.`);
    const request = this.httpClient.post(url, {}).pipe(
      tap(() => {
        this.log('info', 'User logged out successfully. Invalidating user authentication.');
        this.invalidateUserAuthentication();
      }),
    );

    return this.executeRequest(request, 'Logging out user', url);
  }

  register(userRegistration: UserRegistrationDto): Observable<any> {
    const url = `${this.configuration.endpoints.identity.account.register}`;
    this.log('info', `Making request to ${url} to register new user.`);

    const request = this.httpClient.post(url, userRegistration).pipe(
      tap(() => {
        this.log('info', 'User registered successfully.');
      }),
    );
    return this.executeRequest(request, 'Registering new user', url);
  }

  forgotPassword(forgotPassword: ForgotPasswordDto) {
    const url = `${this.configuration.endpoints.identity.account.passwordForgot}`;
    this.log('info', `Making request to ${url} for forgot password.`);

    const request = this.httpClient.post(url, forgotPassword).pipe(
      tap(() => {
        this.log('info', 'Password reset request successful.');
      }),
    );
    return this.executeRequest(request, 'Requesting password reset', url);
  }

  resetPassword(resetPassword: ResetPasswordDto) {
    const url = `${this.configuration.endpoints.identity.account.passwordReset}`;
    this.log('info', `Making request to ${url} to reset password.`);

    const request = this.httpClient
      .post(`${this.configuration.endpoints.identity.account.passwordReset}`, resetPassword)
      .pipe(
        tap(() => {
          this.log('info', 'Password reset successful.');
        }),
      );
    return this.executeRequest(request, 'Resetting password', url);
  }

  isSignInPath(url: string): boolean {
    return (
      this.configuration.endpoints.identity.signIn.individual.includes(url) ||
      this.configuration.endpoints.identity.signIn.facebook.includes(url) ||
      this.configuration.endpoints.identity.signIn.google.includes(url)
    );
  }

  invalidateUserAuthentication(): void {
    this.userIdentitySubject.next(this.invalidUserIdentity);
  }
}
