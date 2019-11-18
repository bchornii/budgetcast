import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  registerForm: FormGroup;
  registrationFailed: boolean;
  registrationMessage: string;

  constructor(private accountService: AccountService,
              private router: Router,
              private toastr: ToastrService) {
    this.registerForm = new FormGroup({
      givenName: new FormControl('', [Validators.required]),
      surName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.email, Validators.required]),
      password: new FormControl('', [Validators.required]),
      passwordConfirm: new FormControl('', [Validators.required])
    });
  }

  register(): void {
    if (this.registerForm.valid) {
      const formValue = this.registerForm.value;
      this.accountService.register(formValue).subscribe(
        _ => {
          this.toastr.success(
            'To finish account registration please' + 
            'follow link in email you should receive.'
          );
          this.router.navigate(['/home']);
        },
        _ => {
          this.registrationFailed = true;
          this.registerForm.markAsPristine();
        }
      );
    }
  }
}
