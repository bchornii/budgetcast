import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

/**
 * HTTP Interceptor that handles:
 * 1. Adding default headers to all requests
 * 2. Handling authentication errors (401/403) with redirect to login
 */
export const httpInterceptor: HttpInterceptorFn = (req, next) => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const router = inject(Router);

  // Add default headers to all requests
  const modifiedReq = req.clone({
    setHeaders: {
      'Content-Type': 'application/json',
      'X-Requested-With': 'XMLHttpRequest',
    },
  });

  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle authentication errors
      if (error.status === 401 || error.status === 403) {
        console.warn('Authentication error detected:', error.status);

        // TODO: Redirect to authentication/login page when route is available
        // Example: router.navigate(['/auth/login']);
        // For now, just log the error
        console.warn('TODO: Redirect to login page - route not implemented yet');

        // Optional: Clear any stored authentication tokens
        // localStorage.removeItem('auth_token');
        // sessionStorage.removeItem('auth_token');
      }

      // Re-throw the error so it can be handled by the calling service
      // and/or trigger toast notifications
      return throwError(() => error);
    }),
  );
};
