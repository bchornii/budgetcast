import {
    HttpHandler,
    HttpInterceptor,
    HttpRequest,
    HttpResponse,
  } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

  @Injectable()
  export class LoggingInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const started = Date.now();
        let ok: string;

        return next.handle(req).pipe(
            tap({
              next: (event: any) => (ok = event instanceof HttpResponse ? 'succeeded' : ''),
              error: () => (ok = 'failed'),
            }),
      
            finalize(() => {              
              if(!environment.production) {
                const elapsed = Date.now() - started;
                const msg = `${req.method} "${req.urlWithParams}" ${ok} in ${elapsed} ms.`;
                console.log('%c[LoggingInterceptor]', 'background: #222; color: #bada55', msg);
              }
            })
          );
    }
  }