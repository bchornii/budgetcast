import { AlertComponent } from './components/alerts/alert/alert.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputComponent } from './components/input/input.component';
import { FormDirective } from './directives/form.directive';
import { FieldsMatchValidatorDirective } from './directives/fields-match.directive';
import { GoogleSigninComponent } from './components/google-signin/google-signin.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { FbSigninComponent } from './components/fb-signin/fb-signin.component';
import { MaterialModule } from './material.module';
import { MatInputComponent } from './components/mat-input/mat-input.component';


@NgModule({
  imports: [
    CommonModule,
    MaterialModule
  ],
  declarations: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    MatInputComponent,

    AlertComponent,
    GoogleSigninComponent,
    SpinnerComponent,
    FbSigninComponent
  ],
  exports: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    MatInputComponent,

    AlertComponent,
    GoogleSigninComponent,
    SpinnerComponent,
    FbSigninComponent
  ]
})
export class SharedModule { }
