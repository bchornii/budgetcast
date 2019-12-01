import { Component, OnInit, Input, ViewChild, ElementRef, Injector, AfterViewInit } from '@angular/core';
import { InputType } from '../input/input.component';
import { FormControl, NgControl, NG_VALUE_ACCESSOR, FormControlName } from '@angular/forms';
import { FormElement } from '../form-element';

@Component({
  selector: 'app-mat-input',
  templateUrl: './mat-input.component.html',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: MatInputComponent,
    multi: true
  }]
})
export class MatInputComponent extends FormElement implements OnInit, AfterViewInit {
  @Input() name = 'item';
  @Input() label: string;
  @Input() readonly = false;
  @Input() type: string = InputType.TEXT;
  @Input() isSearch = false;
  @Input() errMsg = 'Invalid input.';

  @ViewChild('input', { static: false }) input: ElementRef;

  hide: boolean;
  innerValue: any;
  inputControl: FormControl;
  ngControl: NgControl;
  defaultValue = '';
  innerType: string = InputType.TEXT;

  constructor(private injector: Injector,
              public elementRef: ElementRef) {
    super(elementRef);
  }

  ngOnInit() {
    this.inputControl = this.injector
      .get(NgControl).control;
    this.innerType = this.type;
    this.setElementId();
  }

  ngAfterViewInit() {
    this.registerInputEvents(this.input.nativeElement);

    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }

  showPassword($event: MouseEvent) {
    $event.stopPropagation();
    $event.preventDefault();

    this.hide = !this.hide;
    this.innerType === 'text'
    ? this.innerType = 'password'
    : this.innerType = 'text';
  }
}
