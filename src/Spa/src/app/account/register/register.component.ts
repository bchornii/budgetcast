import { ToastrService } from 'ngx-toastr';
import { Component } from '@angular/core';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { UserRegistration } from '../models/user-registration';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  registrationFailed: boolean;
  registrationMessage: string;
  registrationModel = new UserRegistration();

  constructor(private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService) {
  }

  register(): void {
    this.accountService.register(this.registrationModel).subscribe(
      _ => {
        this.toastr.success(
          'To finish account registration please' +
          'follow link in email you should receive.'
        );
        this.router.navigate(['/home']);
      },
      _ => {
        this.registrationFailed = true;
      }
    );
  }
}
