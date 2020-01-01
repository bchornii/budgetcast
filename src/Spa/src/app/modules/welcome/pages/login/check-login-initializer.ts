import { AccountService } from '../../../../services/account.service';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';

export function checkIfUserIsAuthenticated(accountService: AccountService) {
  return () => {
    return accountService.checkUserAuthenticationStatus().pipe(
      catchError(_ => {
        console.error('Cannot retrieve information about user.');
        return of(null);
      }))
      .toPromise();
  };
}
