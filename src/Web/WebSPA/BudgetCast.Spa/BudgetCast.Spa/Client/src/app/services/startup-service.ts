import { catchError, tap, flatMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { ConfigurationService } from './configuration-service';
import { AuthService } from 'src/app/services/auth.service';

export function appStartInitializer(configService: ConfigurationService, authService: AuthService) {
  return () => {
    return configService.load().pipe(
      flatMap(_ => authService.checkUserAuthenticationStatus()),
      catchError(_ => {
        console.error('Cannot retrieve information about user.');
        return of(null);
      }))
      .toPromise();
  };
}
