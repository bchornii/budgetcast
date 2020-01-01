import { ToastrService } from 'ngx-toastr';
import { Component, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { SpinnerComponent } from 'src/app/modules/shared/components/spinner/spinner.component';
import { finalize } from 'rxjs/operators';
import { UserProfile } from 'src/app/models/user-profile';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent {

  profileForm: FormGroup;
  profileModel = new UserProfile();

  @ViewChild(SpinnerComponent, { static: true }) spinner: SpinnerComponent;

  constructor(private accountService: AccountService,
              private router: Router,
              private toastr: ToastrService) {
    this.profileModel = ({ ...accountService.userIdentity });
  }

  updateProfile(): void {
    this.spinner.show();
    this.accountService.updateProfile(this.profileModel).pipe(
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
