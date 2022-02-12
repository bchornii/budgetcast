import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { catchError, filter, finalize, switchMap, take, tap } from 'rxjs/operators';
import { ResponseStatus } from 'src/app/util/constants/response-status';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LocalStorageService } from '../services/local-storage.service';
import { AccessTokenItem } from '../util/constants/auth-constants';
import { BehaviorSubject, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpConfigInterceptor implements HttpInterceptor {

  private isRefreshTokenInProgress = false;
  private accessTokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(private authService: AuthService,
              private router: Router,
              private toastr: ToastrService,
              private localStorage: LocalStorageService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    req = this.addHeaders(req);

    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === ResponseStatus.UNAUTHORIZED || 
            err.status === ResponseStatus.FORBIDDEN) {
            // No need to try to renew access if we get 401 or 403
            // from a sing in endpoint
            if(this.authService.isSignInPath(req.url) ||
               this.authService.isRefreshTokenPath(req.url)) {
                 
                if(this.isRefreshTokenInProgress) {
                   this.isRefreshTokenInProgress = false;
                   this.accessTokenSubject.next(null);
                }
                 
                this.authService.invalidateUserAuthentication();
                this.router.navigate(['/account/login']);
                this.toastr.error('Please sign in.');

            } else {
              if(this.isRefreshTokenInProgress){
                return this.accessTokenSubject.pipe(
                  filter(token => token != null),
                  take(1),
                  switchMap(() => next.handle(this.addHeaders(req)))
                );
              } else {
                this.isRefreshTokenInProgress = true;
                this.accessTokenSubject.next(null);

                const token: string = this.localStorage.getItem(AccessTokenItem);

                if(token) {
                  return this.authService.refreshAccessToken({ accessToken: token}).pipe(
                    switchMap(vm => {
                      this.accessTokenSubject.next(vm.accessToken);
                      return next.handle(this.addHeaders(req));
                    }),
                    finalize(() => this.isRefreshTokenInProgress = false)
                  );
                }
              }
            }            
          }

          if(err.status == ResponseStatus.ERROR) {
            this.toastr.error('Something went wrong.');
          }        
      })
    );
  }

  private addHeaders(req: HttpRequest<any>): HttpRequest<any> {

    const token: string = this.localStorage.getItem(AccessTokenItem);

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

    if(req.headers.has('Accept')) {
      req = req.clone({
        headers: req.headers.set('Accept', 'application/json')
      });
    }

    req = req.clone({
      withCredentials: true
    });

    return req;
  }
}
