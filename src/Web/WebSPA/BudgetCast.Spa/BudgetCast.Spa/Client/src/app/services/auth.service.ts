import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, from, Observable, of } from 'rxjs';
import { tap, catchError, mergeMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { UserIdentity } from '../models/user-identity-vm';
import { UserLoginDto } from '../models/user-login-dto';
import { UserRegistrationDto } from '../models/user-registration-dto';
import { ForgotPasswordDto } from '../models/forgot-password-dto';
import { ResetPasswordDto } from '../models/reset-password-dto';
import { BaseService } from './base-data.service';
import { ConfigurationService } from './configuration-service';
import { CookieService } from 'ngx-cookie-service';
import { StorageService } from './storage.service';
import { AccessTokenItem, XToken } from '../util/constants/auth-constants';
import { UserLoginVm } from '../models/user-login-vm';
import { RefreshAccessTokenVm } from '../models/refresh-access-token-vm';
import { RefreshAccessTokenDto } from '../models/refresh-access-token-dto';

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
              private storageService: StorageService) {
    super();
  }

  verifyIfTokenPassedOnRedirectFromExternalIdp() : Observable<string> {

    var xToken = this.cookieService.get(XToken);

    if (xToken) {
      this.cookieService.delete(XToken);
      this.storageService.setItem(AccessTokenItem, xToken);
    }

    return of(xToken);
  }

  replaceStoredAccessTokenWith(accessToken: string) : Observable<string> {    

    // as a safety net - remove token from cookies (although should not exist at this point, but still)      
    this.cookieService.delete(XToken);

    // update value in local storage for futher usage
    this.storageService.setItem(AccessTokenItem, accessToken);

    return of(accessToken);
  }

  checkUserAuthenticationStatus(): Observable<UserIdentity> {
    return this.httpClient.get<UserIdentity>(
      `${this.configService.endpoints.identity.account.isAuthenticated}`).pipe(tap(r => {
        this.userIdentitySubject.next(r);
      }));
  }

  invalidateUserAuthentication(): void {
    this.storageService.removeItem(AccessTokenItem);
    this.userIdentitySubject.next(this.invalidUserIdentity);
  }

  login(userLogin: UserLoginDto): Observable<any> {
    return this.httpClient.post<UserLoginVm>(`${this.configService.endpoints.identity.signIn.individual}`, userLogin).pipe(
      tap(userLoginVm => this.replaceStoredAccessTokenWith(userLoginVm.accessToken)),
      mergeMap(_ => this.checkUserAuthenticationStatus()),
      catchError(this.handleError)
    );
  }

  refreshAccessToken(refreshToken: RefreshAccessTokenDto): Observable<RefreshAccessTokenVm> {
    return this.httpClient.post<RefreshAccessTokenVm>(`${this.configService.endpoints.identity.signIn.refreshAccessToken}`, refreshToken).pipe(
      tap(vm => this.replaceStoredAccessTokenWith(vm.accessToken)),
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

  register(userRegistration: UserRegistrationDto) : Observable<any> {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.register}`, userRegistration).pipe(
      catchError(this.handleError)
    );
  }

  forgotPassword(forgotPassword: ForgotPasswordDto) {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.passwordForgot}`, forgotPassword).pipe(
        catchError(this.handleError)
      );
  }

  resetPassword(resetPassword: ResetPasswordDto) {
    return this.httpClient.post(
      `${this.configService.endpoints.identity.account.passwordReset}`, resetPassword).pipe(
        catchError(this.handleError)
      );
  }

  isSignInPath(url: string): boolean {
    return this.configService.endpoints.identity.signIn.individual.includes(url) ||
           this.configService.endpoints.identity.signIn.facebook.includes(url) ||
           this.configService.endpoints.identity.signIn.google.includes(url);
  }

  isRefreshTokenPath(url: string): boolean {
    return this.configService.endpoints.identity.signIn.refreshAccessToken.includes(url);
  }
}
