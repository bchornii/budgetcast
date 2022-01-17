import { catchError, tap, flatMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { ConfigurationService } from './configuration-service';
import { AuthService } from 'src/app/services/auth.service';
import { environment } from 'src/environments/environment';

export function appStartInitializer(configService: ConfigurationService, authService: AuthService) {
  return () => {

    if(environment.name === 'nohost') {
      const configFile = `assets/config/config.${environment.name}.json`;
      return configService.load(configFile).pipe(
        flatMap(_ => authService.checkUserAuthenticationStatus()),
        catchError(_ => {
          console.error('Cannot retrieve information about user.');
          return of(null);
        }))
        .toPromise();
    }

    const baseURI = environment.production
          ? this.getBaseUri()
          : environment.devBaseUrl;

    return configService.load(baseURI).pipe(
      flatMap(_ => authService.checkUserAuthenticationStatus()),
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
