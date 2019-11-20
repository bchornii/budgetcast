import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IsInputInvalidDirective } from './directives/is-input-invalid.directive';
import { InputComponent } from './components/input/input.component';
import { FormDirective } from './directives/form.directive';



@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    IsInputInvalidDirective,
    FormDirective,

    InputComponent
  ],
  exports: [
    IsInputInvalidDirective,
    FormDirective,

    InputComponent
  ]
})
export class SharedModule { }
