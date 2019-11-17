import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  forgotPasswordSubmitted: boolean;
  forgotPasswordError: boolean;

  constructor(private accountService: AccountService) {
    this.forgotPasswordForm = new FormGroup({
      email: new FormControl('', [Validators.email, Validators.required])
    });
  }

  forgotPassword(): void {
    if (this.forgotPasswordForm.valid) {
      this.accountService.forgotPassword(this.forgotPasswordForm.value)
        .subscribe(
          _ => this.forgotPasswordSubmitted = true,
          _ => this.forgotPasswordError = true
        );
    }
  }
}
