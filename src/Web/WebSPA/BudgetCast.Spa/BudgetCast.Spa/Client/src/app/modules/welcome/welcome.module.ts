import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { WelcomeRoutingModule } from './welcome-routing.module';
import { WelcomeComponent } from './pages/welcome/welcome.component';

@NgModule({
  declarations: [
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    WelcomeComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    WelcomeRoutingModule,

    SharedModule
  ]
})
export class WelcomeModule { }
