import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  loginForm: FormGroup;
  invalidCredentials: boolean;

  constructor(private accountService: AccountService,
              private router: Router) {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required])
    });
  }

  login(): void {
    if (this.loginForm.valid) {
      const formValue = this.loginForm.value;
      this.accountService.login(formValue).subscribe(
        _ => this.router.navigate(['/home']),
        _ => {
          this.invalidCredentials = true;
          this.loginForm.markAsPristine();
        });
    }
  }

  googleLogin() {
    this.accountService.googleLogin();
  }
}
