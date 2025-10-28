import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.html',
  styles: [],
  imports: [RouterOutlet],
})
export class WelcomeComponent {
  constructor() {}
}
