import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { catchError, firstValueFrom, mergeMap, tap } from 'rxjs';
import { Auth } from './core/auth/services/auth';
import { httpInterceptor } from './core/interceptors/http-interceptor';
import { Configuration } from './core/services/configuration/configuration';

import { routes } from './app.routes';
import { EnvironmentService } from './core/services/environment.service';

/**
 * App initializer function that loads configuration before the app starts
 */
function initializeApp(configuration: Configuration, auth: Auth, environment: EnvironmentService) {
  return async () => {
    let url: string;

    if (environment.name === 'nohost') {
      url = '/assets/config/config.nohost.json';
      console.log(`Loading configuration from local file: ${url}`);
    } else {
      const baseUri = environment.production ? getBaseUri() : environment.devBaseUrl;
      url = `${baseUri}/api/configs/endpoints`;
      console.log(
        `Loading configuration from backend: ${url} (production: ${environment.production})`,
      );
    }

    const config$ = configuration.load(url).pipe(
      tap(() => console.log('Configuration loaded successfully')),
      mergeMap(() => {
        console.log('Checking user authentication status...');
        return auth.checkUserAuthenticationStatus();
      }),
      tap(() => console.log('Authentication status checked')),
      catchError((error) => {
        console.error('Error during app initialization:', error);
        throw error;
      }),
    );

    await firstValueFrom(config$);
    console.log('App initialization complete');
  };
}

function getBaseUri() {
  const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
  return `${baseURI}api/Configs/endpoints`;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([httpInterceptor])),
    provideAppInitializer(() => {
      const configuration = inject(Configuration);
      const auth = inject(Auth);
      const environment = inject(EnvironmentService);
      return initializeApp(configuration, auth, environment)();
    }),
  ],
};
