import { FormGroup, FormControl } from '@angular/forms';

let id = 0;

export function getNewId(): number {
	return ++id;
}

export function validateAllFormFields(formGroup: FormGroup) {
  Object.keys(formGroup.controls).forEach(field => {
    const control = formGroup.get(field);
    if (control instanceof FormControl) {
      control.markAsTouched({ onlySelf: true });
    } else if (control instanceof FormGroup) {
      this.validateAllFormFields(control);
    }
  });
}

export function isString (value) {
  return typeof value === 'string' || value instanceof String;
};

export function isObject (value) {
  return value && typeof value === 'object' && value.constructor === Object;
};

export function isArray (value) {
  return Array.isArray(value);
}

export function isNull (value) {
  return value === null;
};
  
export function isUndefined (value) {
  return typeof value === 'undefined';
};

export class KeyValuePair {    
  constructor(public key: any, public value: any) {}
}