import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewInit, Self } from '@angular/core';
import { InputType } from '../input/input.component';
import { FormControl, NgControl } from '@angular/forms';
import { FormElement } from '../form-element';

@Component({
  selector: 'app-mat-input',
  templateUrl: './mat-input.component.html'
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
  defaultValue = '';
  innerType: string = InputType.TEXT;

  constructor(public elementRef: ElementRef,
              @Self() private ngCrtl: NgControl) {
    super(elementRef);
  }

  ngOnInit() {
    this.ngCrtl.valueAccessor = this;
    this.inputControl = this.ngCrtl
      .control as FormControl;
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
