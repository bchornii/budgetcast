import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IsInputInvalidDirective } from './directives/is-input-invalid.directive';
import { InputComponent } from './components/input/input.component';
import { FormDirective } from './directives/form.directive';
import { FieldsMatchValidatorDirective } from './directives/fields-match.directive';



@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    IsInputInvalidDirective,
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent
  ],
  exports: [
    IsInputInvalidDirective,
    FormDirective,
    FieldsMatchValidatorDirective,

    InputComponent
  ]
})
export class SharedModule { }
