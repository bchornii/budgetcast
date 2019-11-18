import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { validateAllFormFields } from '../../util/util';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm: FormGroup;
  forgotPasswordError: boolean;

  constructor(private accountService: AccountService,
              private router: Router,
              private toasrt: ToastrService,
              private fb: FormBuilder) { }

  ngOnInit() {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.email, Validators.required]]
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
    } else {
        validateAllFormFields(this.forgotPasswordForm);
    }
  }

  isInvalid(controlName: string): boolean {
    return this.forgotPasswordForm.get(controlName).invalid &&
      this.forgotPasswordForm.get(controlName).touched;
  }
}
