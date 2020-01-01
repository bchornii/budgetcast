import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';

@Injectable({
  providedIn: 'root'
})
export class UserAuthenticatedGuard implements CanActivate {
  constructor(private accountService: AccountService,
              private router: Router) { }

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const isAuthenticated = this.accountService
      .userIdentity.isAuthenticated;

    if (!isAuthenticated) {
      this.router.navigate(['/welcome']);
      return false;
    }
    return isAuthenticated;
  }
}
