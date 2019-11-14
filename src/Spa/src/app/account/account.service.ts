import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError, flatMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { UserIdentity } from './models/check-login.model';
import { UserProfile } from './models/user-profile';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private invalidUserIdentity = new UserIdentity();
  private userIdentitySnapshot: UserIdentity;
  private userIdentitySubject = new BehaviorSubject<UserIdentity>(this.invalidUserIdentity);

  userIdentity$ = this.userIdentitySubject.asObservable().pipe(
    tap(userIdentity => this.userIdentitySnapshot = userIdentity)
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
    return this.userIdentitySnapshot.isAuthenticated;
  }

  get userIdentity(): UserIdentity {
    return this.userIdentitySnapshot;
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

  updateProfile(userProfile: UserProfile): Observable<any> {
    return this.httpClient.post(
      `${environment.api.accountApi.updateProfile}`, userProfile).pipe(
        flatMap(_ => this.checkUserAuthenticationStatus()),
        catchError(this.handleError)
      );
  }

  private handleError(err: any) {
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    let errorMessage: string;
    if (err.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMessage = `Backend returned code ${err.status}: ${err.body.error}`;
    }
    console.error(err);
    return throwError(errorMessage);
  }
}
