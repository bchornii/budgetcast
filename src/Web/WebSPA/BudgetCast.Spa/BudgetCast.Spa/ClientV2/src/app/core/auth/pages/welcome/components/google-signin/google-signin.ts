import { ChangeDetectionStrategy, Component, output } from '@angular/core';

@Component({
  selector: 'app-google-signin',
  templateUrl: './google-signin.html',
  styleUrls: ['./google-signin.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GoogleSigninComponent {
  signIn = output();

  onSignIn() {
    this.signIn.emit();
  }
}
