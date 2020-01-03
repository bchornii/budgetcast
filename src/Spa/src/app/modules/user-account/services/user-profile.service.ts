import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserProfile } from '../models/user-profile';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { flatMap, catchError } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth.service';
import { BaseService } from 'src/app/services/base-data.service';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService extends BaseService {

  constructor(private httpClient: HttpClient,
              private authService: AuthService) {
    super();
  }

  updateProfile(userProfile: UserProfile): Observable<any> {
    return this.httpClient.post(
      `${environment.api.accountApi.updateProfile}`, userProfile).pipe(
        flatMap(_ => this.authService.checkUserAuthenticationStatus()),
        catchError(this.handleError)
      );
  }
}
