import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AccountService } from '../../../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { ForgotPassword } from 'src/app/models/forgot-password';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styles: [`
    .heading {
      text-align: center;
    }

    .page {
      width: 300px;
    }

    .app-form-title-large {
      margin-bottom: 0;
    }
  `]
})
export class ForgotPasswordComponent {

  forgotPasswordModel = new ForgotPassword();

  forgotPasswordForm: FormGroup;
  forgotPasswordError: boolean;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private toasrt: ToastrService) { }

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
