import { HttpClient } from '@angular/common/http';
import { DOCUMENT, inject, Injectable } from '@angular/core';
import { BehaviorSubject, mergeMap, Observable, tap } from 'rxjs';
import { BaseService } from '../../services/base-service';
import { Configuration } from '../../services/configuration/configuration';
import { ForgotPasswordDto } from '../models/forgot-password-dto';
import { ResetPasswordDto } from '../models/reset-password-dto';
import { UserIdentity } from '../models/user-identity-vm';
import { UserLoginDto } from '../models/user-login-dto';
import { UserLoginVm } from '../models/user-login-vm';
import { UserRegistrationDto } from '../models/user-registration-dto';

@Injectable({
  providedIn: 'root',
})
export class Auth extends BaseService {
  private retryCount = 2;
  private retryDelay = 500;

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
    const url = `${this.configuration.endpoints.identity.account.isAuthenticated}`;
    const request = this.httpClient.get<UserIdentity>(url).pipe(
      tap((r) => {
        this.userIdentitySubject.next(r);
      }),
      this.retryRequest(this.retryCount, this.retryDelay),
    );

    return this.executeRequest(request, 'Checking user authentication status', url);
  }

  login(userLogin: UserLoginDto): Observable<any> {
    const url = `${this.configuration.endpoints.identity.signIn.individual}`;

    const request = this.httpClient
      .post<UserLoginVm>(`${this.configuration.endpoints.identity.signIn.individual}`, userLogin)
      .pipe(
        tap((userLoginVm) => {
          this.log('info', 'User logged in successfully: ' + JSON.stringify(userLoginVm));
        }),
        mergeMap(() => this.checkUserAuthenticationStatus()),
        this.retryRequest(this.retryCount, this.retryDelay),
      );

    return this.executeRequest(request, 'Logging in user', url);
  }

  googleLogin(): void {
    this.document.location.href = `${this.configuration.endpoints.identity.signIn.google}`;
  }

  facebookLogin(): void {
    this.document.location.href = `${this.configuration.endpoints.identity.signIn.facebook}`;
  }

  logout() {
    const url = `${this.configuration.endpoints.identity.signOut.all}`;
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
    const request = this.httpClient.post(url, userRegistration).pipe(
      tap(() => {
        this.log('info', 'User registered successfully.');
      }),
      this.retryRequest(this.retryCount, this.retryDelay),
    );
    return this.executeRequest(request, 'Registering new user', url);
  }

  forgotPassword(forgotPassword: ForgotPasswordDto) {
    const url = `${this.configuration.endpoints.identity.account.passwordForgot}`;
    const request = this.httpClient.post(url, forgotPassword).pipe(
      tap(() => {
        this.log('info', 'Password reset request successful.');
      }),
      this.retryRequest(this.retryCount, this.retryDelay),
    );
    return this.executeRequest(request, 'Requesting password reset', url);
  }

  resetPassword(resetPassword: ResetPasswordDto) {
    const url = `${this.configuration.endpoints.identity.account.passwordReset}`;
    const request = this.httpClient
      .post(`${this.configuration.endpoints.identity.account.passwordReset}`, resetPassword)
      .pipe(
        tap(() => {
          this.log('info', 'Password reset successful.');
        }),
        this.retryRequest(this.retryCount, this.retryDelay),
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
