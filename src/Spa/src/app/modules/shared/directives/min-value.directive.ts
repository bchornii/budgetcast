import { Validator, NG_VALIDATORS, AbstractControl } from '@angular/forms';
import { Input, Directive } from '@angular/core';
import { minValue } from '../validators/min-validator';

@Directive({
    selector: '[minValue]',
    providers: [{ 
        provide: NG_VALIDATORS, 
        useExisting: MinValueDirective, 
        multi: true }]
})
export class MinValueDirective implements Validator {
    @Input('minValue') min: number;

    validate(control: AbstractControl): {[key: string]: any} {
        return minValue(this.min)(control);
    };
}