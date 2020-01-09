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
import { ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteComponent } from './components/mat-autocomplete/mat-autocomplete.component';
import { MatChipsAutocompleteComponent } from './components/mat-chips-autocomplete/mat-chips-autocomplete.component';
import { MatDatepickerComponent } from './components/mat-datepicker/mat-datepicker.component';
import { NgxCurrencyModule } from "ngx-currency";
import { MatSelectComponent } from './components/mat-select/mat-select.component';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    NgxCurrencyModule,

    ReactiveFormsModule
  ],
  declarations: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    MatInputComponent,

    AlertComponent,
    GoogleSigninComponent,
    SpinnerComponent,
    FbSigninComponent,
    MatAutocompleteComponent,
    MatChipsAutocompleteComponent,
    MatDatepickerComponent,
    MatSelectComponent,
  ],
  exports: [
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent,
    MatInputComponent,

    AlertComponent,
    GoogleSigninComponent,
    SpinnerComponent,
    FbSigninComponent,
    MatAutocompleteComponent,
    MatChipsAutocompleteComponent,
    MatDatepickerComponent,
    MatSelectComponent,

    MaterialModule
  ]
})
export class SharedModule { }
