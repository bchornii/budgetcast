import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { tap } from 'rxjs/operators';
import { ResponseStatus } from 'src/app/util/constants/response-status';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LocalStorageService } from '../services/local-storage.service';
import { AccessTokenItem } from '../util/constants/auth-constants';

@Injectable({
  providedIn: 'root'
})
export class HttpConfigInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService,
              private router: Router,
              private toastr: ToastrService,
              private localStorage: LocalStorageService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

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

    req = req.clone({
      headers: req.headers.set('Accept', 'application/json')
    });

    // TODO: remove hardcoded tenant value
    req = req.clone({
      headers: req.headers.set('tenant', '7064')
    });

    req = req.clone({
      withCredentials: true
    });

    return next.handle(req).pipe(
      tap(null, (err: HttpErrorResponse) => {
          if (err.status === ResponseStatus.UNAUTHORIZED ||
              err.status === ResponseStatus.FORBIDDEN) {
            this.authService.invalidateUserAuthentication();
            this.router.navigate(['/account/login']);
            this.toastr.error('Please sign in.');
          }
          if(err.status == ResponseStatus.ERROR) {
            this.toastr.error('Something went wrong.');
          }
        })
    );
  }
}
