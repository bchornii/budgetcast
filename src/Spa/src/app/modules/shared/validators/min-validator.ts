import { ValidatorFn, FormControl } from '@angular/forms';

export function minValue(value: number): ValidatorFn {
    return (control: FormControl): {[key: string] : any} => {
        const actualValue = control.value;

        if (actualValue <= value) {
            return {
                'min': 'Value is less than allowed.'
            }
        }
        return null;
    }
}