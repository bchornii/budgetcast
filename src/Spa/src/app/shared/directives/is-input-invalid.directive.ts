import { Directive, HostBinding, Input } from '@angular/core';

@Directive({
  selector: '[appIsInputInvalid]'
})
export class IsInputInvalidDirective {
  @HostBinding('class.app-input-control-invalid') private invalid: boolean;
  @Input('appIsInputInvalid') set isInValid(invalid: boolean) {
    this.invalid = invalid;
  }
}
