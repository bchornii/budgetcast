import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { validateAllFormFields } from '../../util/util';
import { ForgotPassword } from '../models/forgot-password';
import { SpinnerComponent } from 'src/app/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {

  forgotPasswordModel = new ForgotPassword();

  forgotPasswordForm: FormGroup;
  forgotPasswordError: boolean;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private toasrt: ToastrService,
    private fb: FormBuilder) { }

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  forgotPassword(): void {
    this.spinner.show();
    this.accountService.forgotPassword(this.forgotPasswordModel).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(
        _ => {
          this.toasrt.success(
            'Please check your email to reset password.');
          this.router.navigate(['/home']);
        },
        _ => this.forgotPasswordError = true
      );
  }
}
