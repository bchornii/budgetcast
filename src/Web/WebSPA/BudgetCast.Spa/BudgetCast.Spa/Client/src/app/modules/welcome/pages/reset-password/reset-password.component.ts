import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { ResetPassword } from 'src/app/models/reset-password';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
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
export class ResetPasswordComponent implements OnInit {
  resetPasswordFailed: boolean;
  resetPasswordParams: {code: string, userId: string};
  resetPasswordModel = new ResetPassword();

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private activatedRoute: ActivatedRoute,
              private authService: AuthService,
              private router: Router,
              private toasrt: ToastrService) {
  }

  ngOnInit() {
    const queryParamMap = this.activatedRoute
      .snapshot.queryParamMap;
    this.resetPasswordParams = {
      code: queryParamMap.get('code').replace(/ /g, "+"),
      userId: queryParamMap.get('userId')
    };
  }

  resetPassword(): void {
    this.spinner.show();
    this.authService.resetPassword({
      ...this.resetPasswordModel,
      ...this.resetPasswordParams
    }).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(
      _ => {
        this.toasrt.success(
          'Your password has been reset.' +
          'You can now login into application.');
        this.router.navigate(['/account/login']);
      },
      _ => {
        this.resetPasswordFailed = true;
      }
    );
  }

}
