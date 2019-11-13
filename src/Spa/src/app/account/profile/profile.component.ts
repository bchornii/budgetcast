import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent{

  profileForm: FormGroup;

  constructor(accountService: AccountService) {
    const userIdentity = accountService.userIdentity;
    this.profileForm = new FormGroup({
      givenName: new FormControl(userIdentity.givenName),
      surName: new FormControl(userIdentity.surName)
    });
  }
}
