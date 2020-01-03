import { UserProfileService } from './../../services/user-profile.service';
import { ToastrService } from 'ngx-toastr';
import { Component, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { UserProfile } from 'src/app/modules/user-account/models/user-profile';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent {

  profileForm: FormGroup;
  profileModel = new UserProfile();

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private userProfileService: UserProfileService,
              private authService: AuthService,
              private router: Router,
              private toastr: ToastrService) {
    this.profileModel = ({ ...authService.userIdentity });
  }

  updateProfile(): void {
    this.spinner.show();
    this.userProfileService.updateProfile(this.profileModel).pipe(
      finalize(() => this.spinner.hide())
    ).subscribe(_ => {
      this.toastr.success('Profile was updated.');
      this.router.navigate(['/home']);
    });
  }

  cancel(): void {
    this.router.navigate(['/home']);
  }
}
