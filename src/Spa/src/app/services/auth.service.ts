import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, catchError, flatMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { UserIdentity } from '../models/check-login.model';
import { UserProfile } from '../modules/user-account/models/user-profile';
import { UserLogin } from '../models/user-login';
import { UserRegistration } from '../models/user-registration';
import { ForgotPassword } from '../models/forgot-password';
import { ResetPassword } from '../models/reset-password';
import { BaseService } from './base-data.service';

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
              private httpClient: HttpClient) {
    super();
  }

  checkUserAuthenticationStatus(): Observable<UserIdentity> {
    return this.httpClient.get<UserIdentity>(
      `${environment.api.accountApi.isAuthenticated}`).pipe(tap(r => {
        this.userIdentitySubject.next(r);
      }));
  }

  invalidateUserAuthentication(): void {
    this.userIdentitySubject.next(this.invalidUserIdentity);
  }

  login(userLogin: UserLogin): Observable<any> {
    return this.httpClient.post(`${environment.api.accountApi.login}`, userLogin).pipe(
      flatMap(_ => this.checkUserAuthenticationStatus()),
      catchError(this.handleError)
    );
  }

  googleLogin(): void {
    this.document.location.href = `${environment.api.accountApi.signInWithGoogle}`;
  }

  facebookLogin(): void {
    this.document.location.href = `${environment.api.accountApi.signInWithFacebook}`;
  }

  logout() {
    return this.httpClient.post(
      `${environment.api.accountApi.logout}`, {}).pipe(
        tap(_ => this.invalidateUserAuthentication())
      );
  }

  register(userRegistration: UserRegistration) : Observable<any> {
    return this.httpClient.post(
      `${environment.api.accountApi.register}`, userRegistration).pipe(
      catchError(this.handleError)
    );
  }

  forgotPassword(forgotPassword: ForgotPassword) {
    return this.httpClient.post(
      `${environment.api.accountApi.forgotPassword}`, forgotPassword).pipe(
        catchError(this.handleError)
      );
  }

  resetPassword(resetPassword: ResetPassword) {
    return this.httpClient.post(
      `${environment.api.accountApi.resetPassword}`, resetPassword).pipe(
        catchError(this.handleError)
      );
  }
}
