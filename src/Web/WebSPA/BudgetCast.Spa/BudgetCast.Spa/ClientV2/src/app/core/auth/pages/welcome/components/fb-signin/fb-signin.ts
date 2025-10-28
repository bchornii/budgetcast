import { ChangeDetectionStrategy, Component, output } from '@angular/core';

@Component({
  selector: 'app-fb-signin',
  templateUrl: './fb-signin.html',
  styleUrls: ['./fb-signin.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FbSigninComponent {
  signIn = output();

  onSignIn() {
    this.signIn.emit();
  }
}
