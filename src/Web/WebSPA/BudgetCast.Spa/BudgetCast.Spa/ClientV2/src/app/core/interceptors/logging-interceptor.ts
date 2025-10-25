import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { EnvironmentService } from '../services/environment.service';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  const environment = inject(EnvironmentService);

  // Skip logging for CORS preflight OPTIONS requests
  if (req.method === 'OPTIONS') {
    return next(req);
  }

  const started = Date.now();
  if (!environment.production) {
    const url = new URL(req.url, window.location.origin);
    const host = url.origin;
    const path = url.pathname + url.search;
    const message = `HTTP Request (${req.method} ${path}) started. Host: ${host}`;
    console.log('%c[LoggingInterceptor]', 'background: #222; color: #bada55', message);
  }

  return next(req).pipe(
    finalize(() => {
      if (!environment.production) {
        const elapsed = Date.now() - started;
        const url = new URL(req.url, window.location.origin);
        const host = url.origin;
        const path = url.pathname + url.search;
        const message = `HTTP Request (${req.method} ${path}) completed in ${elapsed}ms. Host: ${host}`;
        console.log('%c[LoggingInterceptor]', 'background: #222; color: #bada55', message);
      }
    }),
  );
};
