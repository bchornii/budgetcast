import { ElementRef } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';
import { getNewId } from '../../core/services/util';

/**
 * Base class for form elements that implement ControlValueAccessor.
 * Provides common functionality for form controls with change detection and event handling.
 *
 * @example
 * ```typescript
 * export class CustomInputComponent extends FormElement {
 *   constructor(elementRef: ElementRef) {
 *     super(elementRef);
 *   }
 * }
 * ```
 */
export class FormElement implements ControlValueAccessor {
  id = '';
  name = '';
  innerValue: unknown = null;
  defaultValue: unknown = null;

  private propagateChange: ((value: unknown) => void) | undefined;
  private propagateTouch: (() => void) | undefined;

  constructor(public elementRef: ElementRef) {}

  /**
   * Handles value changes and propagates them to Angular forms
   */
  onChange(value: string): void {
    if (this.innerValue !== value) {
      this.innerValue = value;
      if (this.propagateChange) {
        this.propagateChange(value);
      }
    }
  }

  /**
   * Handles blur events and marks the control as touched
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onBlur(_$event?: Event): void {
    if (this.propagateTouch) {
      this.propagateTouch();
    }
  }

  /**
   * Handles focus events - override in child classes if needed
   */
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onFocus(_$event: Event): void {
    // Override in child components if needed
  }

  /**
   * Writes a new value to the element (ControlValueAccessor interface)
   */
  writeValue(value: unknown): void {
    this.innerValue = value ?? this.defaultValue;
  }

  /**
   * Registers a callback function for value changes (ControlValueAccessor interface)
   */
  registerOnChange(fn: (value: unknown) => void): void {
    this.propagateChange = fn;
  }

  /**
   * Registers a callback function for touch events (ControlValueAccessor interface)
   */
  registerOnTouched(fn: () => void): void {
    this.propagateTouch = fn;
  }

  /**
   * Sets a unique element ID based on the name property
   */
  setElementId(): void {
    this.id = [this.name, getNewId().toString()].join('-');
  }

  /**
   * Registers input events on an HTML input element
   * @param element - The HTML input element to register events on
   */
  registerInputEvents(element: HTMLInputElement): void {
    element.onchange = () => this.onChange(element.value);
    element.oncut = () => this.onChange(element.value);
    element.onpaste = () => this.onChange(element.value);
    element.onkeyup = () => this.onChange(element.value);
  }
}
