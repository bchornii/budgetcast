import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IsInputInvalidDirective } from './directives/is-input-invalid.directive';



@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    IsInputInvalidDirective
  ],
  exports: [
    IsInputInvalidDirective
  ]
})
export class SharedModule { }
