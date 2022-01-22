import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, flatMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { UserIdentity } from '../models/check-login.model';
import { UserLogin } from '../models/user-login';
import { UserRegistration } from '../models/user-registration';
import { ForgotPassword } from '../models/forgot-password';
import { ResetPassword } from '../models/reset-password';
import { BaseService } from './base-data.service';
import { ConfigurationService } from './configuration-service';
import { CookieService } from 'ngx-cookie-service';
import { LocalStorageService } from './local-storage.service';
import { AccessTokenItem, XToken } from '../util/constants/auth-constants';
import { UserLoginVm } from '../models/user-login-vm';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {

  private invalidUserIdentity = new UserIdentity();
  private userIdentitySubject = new BehaviorSubject<UserIdentity>(this.invalidUserIdentity);

  get isUserValid(): boolean { return this.userIdentity.isAuthenticated; }
  get userIdentity(): UserIdentity { return this.userIdentitySubject.value; }

  userIdentity$ = this.userIdentitySubject.asObservable();

  constructor(@Inject(DOCUMENT)
              private document: Document,
              private httpClient: HttpClient,
              private configService: ConfigurationService,
              private cookieService: CookieService,
              private localStorage: LocalStorageService) {
    super();
  }

  verifyIfTokenPassedAfterRedirect() : Observable<string> {
    var token = this.cookieService.get(XToken);
    if (token) {
      this.cookieService.delete(XToken);
      this.localStorage.setItem(AccessTokenItem, token);
    }
    return of(token);
  }

  checkUserAuthenticationStatus(accessToken?: string): Observable<UserIdentity> {

    // Check for token passed from successful external login
    var xToken = this.cookieService.get(XToken);
    if (xToken) {
      this.cookieService.delete(XToken);
      this.localStorage.setItem(AccessTokenItem, xToken);
    }

    // Check for token passed as a result of successful individual login
    if (accessToken){
      this.cookieService.delete(XToken);
      this.localStorage.setItem(AccessTokenItem, accessToken);
    }

    return this.httpClient.get<UserIdentity>(
      `${this.configService.endpoints.identity.account.isAuthenticated}`).pipe(tap(r => {
        this.userIdentitySubject.next(r);
      }));
  }

  invalidateUserAuthentication(): void {
    this.localStorage.removeItem(AccessTokenItem);
    this.userIdentitySubject.next(this.invalidUserIdentity);
  }

  login(userLogin: UserLogin): Observable<any> {
    return this.httpClient.post<UserLoginVm>(`${this.configService.endpoints.identity.signIn.individual}`, userLogin).pipe(
      flatMap(userLoginVm => this.checkUserAuthenticationStatus(userLoginVm.accessToken)),
      catchError(this.handleError)
    );
  }

  googleLogin(): void {
    this.document.location.href = `${this.configService.endpoints.identity.signIn.google}`;
  }

  facebookLogin(): void {
    this.document.location.href = `${this.configService.endpoints.identity.signIn.facebook}`;
  }

  logout() {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.signOut.all}`, {}).pipe(
        tap(_ => this.invalidateUserAuthentication())
      );
  }

  register(userRegistration: UserRegistration) : Observable<any> {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.register}`, userRegistration).pipe(
      catchError(this.handleError)
    );
  }

  forgotPassword(forgotPassword: ForgotPassword) {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.passwordForgot}`, forgotPassword).pipe(
        catchError(this.handleError)
      );
  }

  resetPassword(resetPassword: ResetPassword) {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.passwordReset}`, resetPassword).pipe(
        catchError(this.handleError)
      );
  }
}
