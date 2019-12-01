import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Self, EventEmitter } from '@angular/core';
import { getNewId } from 'src/app/util/util';

export class MatFormElement implements ControlValueAccessor {
  id: string;
  name: string;
  inputControl: FormControl;
  valueChangesSubstription: Subscription;
  onBlurChange: EventEmitter<Event>;
  onFocusChange: EventEmitter<Event>;

  propagateTouch: () => {};

  constructor(@Self() public ngCrtl: NgControl) {
    this.ngCrtl.valueAccessor = this;
    this.inputControl = new FormControl('', this.ngCrtl.validator);
  }

  onBlur($event?: Event) {
    this.propagateTouch();

    if (this.onBlurChange) {
      this.onBlurChange.emit($event);
    }
  }

  onFocus($event: Event) {
    if (this.onFocusChange) {
      this.onFocusChange.emit($event);
    }
  }

  writeValue(value: any) {
    if (value) {
      this.inputControl.setValue(value, { emitEvent: false});
    }
  }

  registerOnChange(fn) {
    this.valueChangesSubstription =
      this.inputControl.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn) {
    this.propagateTouch = fn;
  }

  setDisabledState(disabled: boolean) {
    disabled ? this.inputControl.disable({ onlySelf: true})
             : this.inputControl.enable({ onlySelf: true});
  }

  setElementId() {
    this.id = [this.name, getNewId().toString()].join('-');
  }
}
