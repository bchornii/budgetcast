import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;  
  forgotPasswordError: boolean;

  constructor(private accountService: AccountService,
              private router: Router,
              private toasrt: ToastrService) {
    this.forgotPasswordForm = new FormGroup({
      email: new FormControl('', [Validators.email, Validators.required])
    });
  }

  forgotPassword(): void {
    if (this.forgotPasswordForm.valid) {
      this.accountService.forgotPassword(this.forgotPasswordForm.value)
        .subscribe(
          _ => {
            this.toasrt.success(
              'Please check your email to reset password.');
            this.router.navigate(['/home']);
          },
          _ => this.forgotPasswordError = true
        );
    }
  }
}
