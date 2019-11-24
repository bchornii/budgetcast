import { Component, ViewChild } from '@angular/core';
import { AccountService } from '../account.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { UserLogin } from '../models/user-login';
import { SpinnerComponent } from 'src/app/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  loginModel = new UserLogin();
  invalidCredentials: boolean;

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private accountService: AccountService,
              private router: Router) {    
  }

  login(): void {    
    this.spinner.show();
    this.accountService.login(this.loginModel).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(
        _ => this.router.navigate(['/home']),
        _ => this.invalidCredentials = true);
  }

  googleLogin() {
    this.accountService.googleLogin();
  }

  fbLogin() {
    this.accountService.facebookLogin();
  }
}
