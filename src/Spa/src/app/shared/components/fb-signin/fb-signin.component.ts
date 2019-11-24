import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-fb-signin',
  templateUrl: './fb-signin.component.html',
  styleUrls: ['./fb-signin.component.scss']
})
export class FbSigninComponent {
  @Output() signIn = new EventEmitter();

  onSignIn() {
    this.signIn.emit();
  }
}
