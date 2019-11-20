import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IsInputInvalidDirective } from './directives/is-input-invalid.directive';
import { InputComponent } from './components/input/input.component';



@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    IsInputInvalidDirective,

    InputComponent
  ],
  exports: [
    IsInputInvalidDirective,
    InputComponent
  ]
})
export class SharedModule { }
