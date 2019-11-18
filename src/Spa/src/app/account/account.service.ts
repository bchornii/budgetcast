import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, catchError, flatMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { UserIdentity } from './models/check-login.model';
import { UserProfile } from './models/user-profile';
import { BaseService } from '../shared/services/base-data.service';
import { UserLogin } from './models/user-login';
import { UserRegistration } from './models/user-registration';
import { ForgotPassword } from './models/forgot-password';
import { ResetPassword } from './models/reset-password';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {

  private invalidUserIdentity = new UserIdentity();
  private userIdentitySnapshot: UserIdentity;
  private userIdentitySubject = new BehaviorSubject<UserIdentity>(this.invalidUserIdentity);

  get isUserValid(): boolean { return this.userIdentitySnapshot.isAuthenticated; }
  get userIdentity(): UserIdentity { return this.userIdentitySnapshot; }

  userIdentity$ = this.userIdentitySubject.asObservable().pipe(
    tap(userIdentity => this.userIdentitySnapshot = userIdentity)
  );

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

  updateProfile(userProfile: UserProfile): Observable<any> {
    return this.httpClient.post(
      `${environment.api.accountApi.updateProfile}`, userProfile).pipe(
        flatMap(_ => this.checkUserAuthenticationStatus()),
        catchError(this.handleError)
      );
  }
}
