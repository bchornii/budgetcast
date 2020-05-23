import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { NgModule } from '@angular/core';
import { WelcomeComponent } from './pages/welcome/welcome.component';

const routes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
    children: [
      {
        path: '',
        component: HomeComponent
      },
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'register',
        component: RegisterComponent
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordComponent
      },
      {
        path: 'reset-password',
        component: ResetPasswordComponent
      }
    ]
  }
];
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class WelcomeRoutingModule {}
