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
  profileModel = new UserProfile();

  constructor(private accountService: AccountService,
              private router: Router,
              private toastr: ToastrService) {
    this.profileModel = accountService.userIdentity;
  }

  updateProfile(): void {
    this.accountService.updateProfile(this.profileModel)
        .subscribe(_ => {
          this.toastr.success('Profile was updated.');
          this.router.navigate(['/home']);
        });
  }

  cancel(): void {
    this.router.navigate(['/home']);
  }
}
