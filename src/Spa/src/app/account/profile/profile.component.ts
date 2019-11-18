import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators, AbstractControl } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { UserProfile } from '../models/user-profile';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent{

  profileForm: FormGroup;

  constructor(private accountService: AccountService,
              private router: Router,
              private toastr: ToastrService) {
    const userIdentity = accountService.userIdentity;
    this.profileForm = new FormGroup({
      givenName: new FormControl(
        userIdentity.givenName,
        [Validators.required, Validators.pattern('[a-zA-Z].*')]
      ),
      surName: new FormControl(
        userIdentity.surName,
        [Validators.required, Validators.pattern('[a-zA-Z].*')]
      )
    });
  }

  updateProfile(): void {
    if (this.profileForm.valid) {
      const formValue = this.profileForm.value;
      this.accountService.updateProfile(formValue)
        .subscribe(_ => {
          this.toastr.success('Profile was updated.');
          this.router.navigate(['/home']);
        });
    }
  }

  cancel(): void {
    this.router.navigate(['/home']);
  }
}
