import { catchError, tap, mergeMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { ConfigurationService } from './configuration-service';
import { AuthService } from 'src/app/services/auth.service';
import { environment } from 'src/environments/environment';

export function appStartInitializer(configService: ConfigurationService, authService: AuthService) {
  return () => {

    if(environment.name === 'nohost') {
      const configFileUrl = `assets/config/config.${environment.name}.json`;

      return configService.load(configFileUrl).pipe(
        tap(_ => authService.verifyIfTokenPassedOnRedirectFromExternalIdp()),
        mergeMap(_ => authService.checkUserAuthenticationStatus()),
        catchError(_ => {
          console.error('Cannot retrieve information about user.');
          return of(null);
        }))
        .toPromise();
    }

    const baseURI = environment.production
          ? getBaseUri()
          : environment.devBaseUrl;

    return configService.load(baseURI).pipe(
      tap(_ => authService.verifyIfTokenPassedOnRedirectFromExternalIdp()),
      mergeMap(_ => authService.checkUserAuthenticationStatus()),
      catchError(_ => {
        console.error('Cannot retrieve information about user.');
        return of(null);
      }))
      .toPromise();
  };

  function getBaseUri() {
    const baseURI = document.baseURI.endsWith('/')
      ? document.baseURI : `${document.baseURI}/`;
    return `${baseURI}api/Configs/endpoints`;
  }
}
