import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { accountRoutes } from './account.routes';
import { LoginComponent } from './login/login.component';

@NgModule({
  declarations: [
    LoginComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(accountRoutes)
  ]
})
export class AccountModule { }
