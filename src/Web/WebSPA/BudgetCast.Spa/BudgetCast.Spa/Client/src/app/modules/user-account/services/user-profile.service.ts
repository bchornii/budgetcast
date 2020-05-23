import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserProfile } from '../models/user-profile';
import { Observable } from 'rxjs';
import { flatMap, catchError } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth.service';
import { BaseService } from 'src/app/services/base-data.service';
import { ConfigurationService } from 'src/app/services/configuration-service';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService extends BaseService {

  constructor(private httpClient: HttpClient,
              private authService: AuthService,
              private configService: ConfigurationService) {
    super();
  }

  updateProfile(userProfile: UserProfile): Observable<any> {
    return this.httpClient.post(
      `${this.configService.endpoints.dashboard.account.updateProfile}`, userProfile).pipe(
        flatMap(_ => this.authService.checkUserAuthenticationStatus()),
        catchError(this.handleError)
      );
  }
}
