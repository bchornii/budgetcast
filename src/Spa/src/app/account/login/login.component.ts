import { Component } from '@angular/core';
import { AccountService } from '../account.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { UserLogin } from '../models/user-login';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  loginModel = new UserLogin();
  invalidCredentials: boolean;

  constructor(private accountService: AccountService,
              private router: Router) {    
  }

  login(): void {    
    this.accountService.login(this.loginModel).subscribe(
        _ => this.router.navigate(['/home']),
        _ => this.invalidCredentials = true);
  }

  googleLogin() {
    this.accountService.googleLogin();
  }
}
