import { Component, ViewChild } from '@angular/core';
import { AuthService } from '../../../../services/auth.service';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { UserLoginDto } from 'src/app/models/user-login-dto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  loginModel = new UserLoginDto();
  invalidCredentials: boolean;

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private authService: AuthService,
              private router: Router) {
  }

  login(): void {
    this.spinner.show();
    this.authService.login(this.loginModel).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(
        _ => this.router.navigate(['/home']),
        _ => this.invalidCredentials = true);
  }

  googleLogin() {
    this.authService.googleLogin();
  }

  fbLogin() {
    this.authService.facebookLogin();
  }
}
