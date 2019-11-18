import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { AccountService } from '../../account/account.service';
import { tap } from 'rxjs/operators';

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    const token: string = localStorage.getItem('token');
    if (token) {
      req = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
    }

    if (!req.headers.has('Content-Type')) {
      req = req.clone({
        headers: req.headers.set('Content-Type', 'application/json')
      });
    }

    req = req.clone({
      headers: req.headers.set('Accept', 'application/json')
    });

    req = req.clone({
      withCredentials: true
    });

    return next.handle(req).pipe(
      tap(_ => _, (err: HttpErrorResponse) => {
          if (err.status === 401 || err.status === 403) {
            this.accountService.invalidateUserAuthentication();
          }
        })
    );
  }
}
