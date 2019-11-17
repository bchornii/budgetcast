import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ProfileComponent } from './profile/profile.component';
import { CanActivateProfileGuard } from './profile/can-activate-profile.guard';
import { RegisterComponent } from './register/register.component';

export const accountRoutes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [CanActivateProfileGuard]
  },
  {
    path: 'register',
    component: RegisterComponent
  }
];
