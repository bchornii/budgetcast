import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(private accountService: AccountService) { }

  login() {
    this.accountService.login();
  }
}
