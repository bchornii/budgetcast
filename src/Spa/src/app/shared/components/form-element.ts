import { ControlValueAccessor } from '@angular/forms';
import { EventEmitter, ElementRef } from '@angular/core';
import { getNewId } from 'src/app/util/util';

export class FormElement implements ControlValueAccessor {
    id: string;
	name: string;
	innerValue: any = null;
	defaultValue: any = null;

    propagateChange: (value: any) => {};
    propagateTouch: () => {};

    onBlurChange: EventEmitter<Event>;
    onFocusChange: EventEmitter<Event>;

    constructor(public elementRef: ElementRef) { }
    
    onChange(value) {
		if (this.innerValue !== value) {
			this.innerValue = value;
			this.propagateChange(value);
		}
    }

    onBlur($event: Event) {
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

    writeValue(value: any): void {
        this.innerValue = value || this.defaultValue;
    }    
    
    registerOnChange(fn: any): void {
        this.propagateChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.propagateTouch = fn;
    }
    
    setElementId() {
		this.id = [this.name, getNewId().toString()].join('-');
	}

    registerInputEvents(element: HTMLInputElement) {
		element.onchange = () => this.onChange(element.value);
		element.oncut 	 = () => this.onChange(element.value);
		element.onpaste  = () => this.onChange(element.value);
		element.onkeyup  = () => this.onChange(element.value);
	}
}