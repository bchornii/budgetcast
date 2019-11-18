import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { MustMatch } from '../validators/must-match.validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  registrationFailed: boolean;
  registrationMessage: string;

  constructor(private accountService: AccountService,
              private router: Router,
              private toastr: ToastrService,
              private fb: FormBuilder) {
  }

  ngOnInit() {
    this.registerForm = this.fb.group({
      givenName: ['', [Validators.required]],
      surName: ['', [Validators.required]],
      email: ['', [Validators.email, Validators.required]],
      password: ['', [Validators.required]],
      passwordConfirm: ['', [Validators.required]]
    }, {
      validator: MustMatch('password', 'passwordConfirm')
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

  isInvalid(controlName: string): boolean {
    return this.registerForm.get(controlName).invalid &&
           this.registerForm.get(controlName).touched;
  }

}
