import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { ResetPassword } from '../models/reset-password';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordFailed: boolean;
  resetPasswordParams: {code: string, userId: string};
  resetPasswordModel = new ResetPassword();

  constructor(private activatedRoute: ActivatedRoute,
              private accountService: AccountService,
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
    this.accountService.resetPassword({
      ...this.resetPasswordModel,
      ...this.resetPasswordParams
    }).subscribe(
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
