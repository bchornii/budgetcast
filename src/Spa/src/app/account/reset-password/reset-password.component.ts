import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordFailed: boolean;
  resetPasswordParams: {code: string, userId: string};
  resetPasswordForm: FormGroup;

  constructor(private activatedRoute: ActivatedRoute,
              private accountService: AccountService,
              private router: Router) {
    this.resetPasswordForm = new FormGroup({
      email: new FormControl('', [Validators.email, Validators.required]),
      password: new FormControl('', [Validators.required]),
      passwordConfirm: new FormControl('', [Validators.required])
    });
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
    if (this.resetPasswordForm.valid) {
      const formValues = this.resetPasswordForm.value;
      this.accountService.resetPassword({
        ...formValues,
        ...this.resetPasswordParams
      }).subscribe(
        _ => this.router.navigate(['/account/login']),
        _ => {
          this.resetPasswordFailed = true;
          this.resetPasswordForm.markAsPristine();
        }
      );
    }
  }

}
