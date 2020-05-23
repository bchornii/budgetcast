import { Directive, Input, HostListener } from "@angular/core";
import { NG_VALIDATORS, Validator, FormGroup, FormControl } from '@angular/forms';

@Directive({
    selector: '[appFieldsMatch]',
    providers: [
        {
            provide: NG_VALIDATORS,
            useExisting: FieldsMatchValidatorDirective,
            multi: true
        }
    ]
})
export class FieldsMatchValidatorDirective implements Validator {
    @Input() fieldName: string;
    @Input() matchFieldName: string;

    formGroup: FormGroup;

    @HostListener('submit')
    onSubmit() {
        if(this.formGroup) {
            this.formGroup.updateValueAndValidity({
                onlySelf: true
            });
        }        
    }

    validate(formGroup: FormGroup): { [key: string]: any } {
        this.formGroup = formGroup;
        const field = formGroup.get(this.fieldName);
        const matchField = formGroup.get(this.matchFieldName);

        if (field && matchField) {
            const notEqual = field.value != matchField.value;
            const fieldDirtyAndTouched = field.dirty && field.touched;
            const matchFieldDirtyAndTouched = matchField.dirty && matchField.touched;

            if (notEqual && fieldDirtyAndTouched && matchFieldDirtyAndTouched) {
                return {
                    'notMatch': 'Field values do not match'
                };
            }
        }
        return null;
    };
}