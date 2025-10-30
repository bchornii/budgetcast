import { Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { Observer, Subscription } from 'rxjs';
import { getNewId } from '../../core/services/util';

/**
 * Base class for Material form elements that implement ControlValueAccessor.
 * Provides common functionality for form controls with validation and change detection.
 */
export class MatFormElement implements ControlValueAccessor {
  id = '';
  name = '';
  inputControl: FormControl;
  valueChangesSubscription: Subscription | undefined;

  private propagateTouch: (() => void) | undefined;

  constructor(@Self() public ngCrtl: NgControl) {
    if (this.ngCrtl) {
      this.ngCrtl.valueAccessor = this;
    }
    this.inputControl = new FormControl('', this.ngCrtl?.validator ?? null);
  }

  /**
   * Handles blur events and marks the control as touched
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onBlur($event?: Event): void {
    if (this.propagateTouch) {
      this.propagateTouch();
    }
  }

  /**
   * Handles focus events - override in child classes if needed
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onFocus($event: Event): void {
    // Override in child components if needed
  }

  /**
   * Writes a new value to the element (ControlValueAccessor interface)
   */
  writeValue(value: unknown): void {
    if (value !== null && value !== undefined) {
      this.inputControl.setValue(value, { emitEvent: false });
    }
  }

  /**
   * Registers a callback function that should be called when the control's value changes
   */
  registerOnChange(fn: Partial<Observer<unknown>> | ((value: unknown) => void) | undefined): void {
    this.valueChangesSubscription = this.inputControl.valueChanges.subscribe(fn);
  }

  /**
   * Registers a callback function that should be called when the control is touched
   */
  registerOnTouched(fn: () => void): void {
    this.propagateTouch = fn;
  }

  /**
   * Sets the disabled state of the form control
   */
  setDisabledState(disabled: boolean): void {
    if (disabled) {
      this.inputControl.disable({ onlySelf: true });
    } else {
      this.inputControl.enable({ onlySelf: true });
    }
  }

  /**
   * Sets a unique element ID based on the name and a generated ID
   */
  setElementId(): void {
    this.id = [this.name, getNewId().toString()].join('-');
  }
}
