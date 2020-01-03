import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserAuthenticatedGuard implements CanActivate {
  constructor(private authService: AuthService,
              private router: Router) { }

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const isAuthenticated = this.authService
      .userIdentity.isAuthenticated;

    if (!isAuthenticated) {
      this.router.navigate(['/welcome']);
      return false;
    }
    return isAuthenticated;
  }
}
