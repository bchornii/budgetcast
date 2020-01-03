import { AuthService } from '../../../../services/auth.service';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';

export function checkIfUserIsAuthenticated(authService: AuthService) {
  return () => {
    return authService.checkUserAuthenticationStatus().pipe(
      catchError(_ => {
        console.error('Cannot retrieve information about user.');
        return of(null);
      }))
      .toPromise();
  };
}
