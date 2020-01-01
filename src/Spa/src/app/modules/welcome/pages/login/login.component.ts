import { Component, ViewChild } from '@angular/core';
import { AccountService } from '../../../../services/account.service';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { UserLogin } from 'src/app/models/user-login';

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
