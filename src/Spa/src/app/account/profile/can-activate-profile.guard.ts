import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AccountService } from '../account.service';

@Injectable({
  providedIn: 'root'
})
export class CanActivateProfileGuard implements CanActivate {

  constructor(private accountService: AccountService,
              private router: Router) {}

  canActivate(): boolean {
    if (!this.accountService.isUserValid) {
      this.router.navigate(['/home']);
    }
    return this.accountService.isUserValid;
  }
}
