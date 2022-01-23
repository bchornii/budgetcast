import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserProfileDto } from '../models/user-profile-dto';
import { Observable } from 'rxjs';
import { flatMap, catchError } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth.service';
import { BaseService } from 'src/app/services/base-data.service';
import { ConfigurationService } from 'src/app/services/configuration-service';
import { UserProfileVm } from '../models/user-profile-vm';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService extends BaseService {

  constructor(private httpClient: HttpClient,
              private authService: AuthService,
              private configService: ConfigurationService) {
    super();
  }

  updateProfile(userProfile: UserProfileDto): Observable<any> {
    return this.httpClient.post<UserProfileVm>(
      `${this.configService.endpoints.identity.account.update}`, userProfile).pipe(
        flatMap(userProfileVm => this.authService.checkUserAuthenticationStatus(userProfileVm.accessToken)),
        catchError(this.handleError)
      );
  }
}
