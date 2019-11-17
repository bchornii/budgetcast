import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;
  registrationFailed: boolean;
  registrationMessage: string;

  constructor(private accountService: AccountService,
              private router: Router) {
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
        _ => this.router.navigate(['/home']),
        _ => {
          this.registrationFailed = true;
          this.registerForm.markAsPristine();
        }
      );
    }
  }
}
