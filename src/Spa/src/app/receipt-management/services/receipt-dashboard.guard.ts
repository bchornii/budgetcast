import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

@Injectable({
  providedIn: 'root'
})
export class ReceiptDashboardGuard implements CanActivate {
  constructor(private accountService: AccountService,
              private router: Router) { }

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const isAuthenticated = this.accountService
      .userIdentity.isAuthenticated;

    if (!isAuthenticated) {
      this.router.navigate(['/home']);
      return false;
    }
    return isAuthenticated;
  }
}
