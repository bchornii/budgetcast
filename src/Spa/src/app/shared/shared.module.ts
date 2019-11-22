import { AlertComponent } from './components/alerts/alert/alert.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './components/input/input.component';
import { FormDirective } from './directives/form.directive';
import { FieldsMatchValidatorDirective } from './directives/fields-match.directive';
import { GoogleSigninComponent } from './components/google-signin/google-signin.component';



@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    AlertComponent,
    GoogleSigninComponent
  ],
  exports: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    AlertComponent,
    GoogleSigninComponent
  ]
})
export class SharedModule { }
