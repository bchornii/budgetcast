import { ToastrService } from 'ngx-toastr';
import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { UserRegistration } from 'src/app/models/user-registration';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: [`
    .heading {
      text-align: center;
    }

    .page {
      width: 300px;
    }

    .app-form-title-large {
      margin-bottom: 0;
    }
  `]
})
export class RegisterComponent {
  registrationFailed: boolean;
  registrationMessage: string;
  registrationModel = new UserRegistration();

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private authService: AuthService,
    private router: Router,
    private toastr: ToastrService) {
  }

  register(): void {
    this.spinner.show();
    this.authService.register(this.registrationModel).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(
      _ => {
        this.toastr.success(
          'To finish account registration please' +
          'follow link in email you should receive.'
        );
        this.router.navigate(['/home']);
      },
      _ => {
        this.registrationFailed = true;
      }
    );
  }
}
