import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-google-signin',
  templateUrl: './google-signin.component.html',
  styleUrls: ['./google-signin.component.scss']
})
export class GoogleSigninComponent {
  @Output() signIn = new EventEmitter();

  onSignIn() {
    this.signIn.emit();
  }

}
