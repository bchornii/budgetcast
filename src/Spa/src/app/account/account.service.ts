import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private isUserAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isUserAuthenticated$ = this.isUserAuthenticatedSubject.asObservable();

  constructor(@Inject(DOCUMENT)
              private document: Document,
              private httpClient: HttpClient) { }

  checkUserAuthenticationStatus(): Observable<boolean> {
    return this.httpClient.get<boolean>(
      `${environment.api.accountApi.isAuthenticated}`).pipe(tap(r => {
        this.isUserAuthenticatedSubject.next(r);
      }));
  }

  invalidateUserAuthentication(): void {
    this.isUserAuthenticatedSubject.next(false);
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
