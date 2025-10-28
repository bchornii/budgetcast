import { FormControl, FormGroup } from '@angular/forms';

let id = 0;

export function getNewId(): number {
  return ++id;
}

export function validateAllFormFields(formGroup: FormGroup): void {
  Object.keys(formGroup.controls).forEach((field) => {
    const control = formGroup.get(field);
    if (control instanceof FormControl) {
      control.markAsTouched({ onlySelf: true });
    } else if (control instanceof FormGroup) {
      validateAllFormFields(control);
    }
  });
}

export function isString(value: any): value is string {
  return typeof value === 'string' || value instanceof String;
}

export function isNumber(value: any): value is number {
  return typeof value === 'number' && isFinite(value);
}

export function isBoolean(value: any): value is boolean {
  return typeof value === 'boolean';
}

export function isObject(value: any): value is object {
  return value && typeof value === 'object' && value.constructor === Object;
}

export function isNull(value: any): value is null {
  return value === null;
}

export function isUndefined(value: any): value is undefined {
  return typeof value === 'undefined';
}

export class KeyValuePair<K, V> {
  constructor(
    public key: K,
    public value: V,
  ) {}
}
