import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { UserIdentity } from './models/check-login.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private invalidUserIdentity = new UserIdentity();
  private isAuthenticated: boolean;
  private userIdentitySubject = new BehaviorSubject<UserIdentity>(this.invalidUserIdentity);

  userIdentity$ = this.userIdentitySubject.asObservable().pipe(
    tap(userIdentity => this.isAuthenticated = userIdentity.isAuthenticated)
  );

  constructor(@Inject(DOCUMENT)
              private document: Document,
              private httpClient: HttpClient) { }

  checkUserAuthenticationStatus(): Observable<UserIdentity> {
    return this.httpClient.get<UserIdentity>(
      `${environment.api.accountApi.isAuthenticated}`).pipe(tap(r => {
        this.userIdentitySubject.next(r);
      }));
  }

  invalidateUserAuthentication(): void {
    this.userIdentitySubject.next(this.invalidUserIdentity);
  }

  get isUserValid(): boolean {
    return this.isAuthenticated;
  }

  login(): void {
    this.document.location.href = `${environment.api.accountApi.signInWithGoogle}`;
  }

  logout() {
    return this.httpClient.post(
      `${environment.api.accountApi.logout}`, {}).pipe(
        tap(_ => this.invalidateUserAuthentication())
      );
  }

  check() {
    return this.httpClient.get(`${environment.api.accountApi.check}`).pipe(
      tap(_ => console.log('check success'), _ => console.log('unsuccess check'))
    );
  }
}
