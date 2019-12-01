import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewInit, Self, OnDestroy, EventEmitter, Output } from '@angular/core';
import { InputType } from '../input/input.component';
import { FormControl, NgControl, ControlValueAccessor } from '@angular/forms';
import { getNewId } from 'src/app/util/util';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-mat-input',
  templateUrl: './mat-input.component.html'
})
export class MatInputComponent implements OnInit, OnDestroy, AfterViewInit, ControlValueAccessor {
  @Input() name = 'item';
  @Input() label: string;
  @Input() readonly = false;
  @Input() type: string = InputType.TEXT;
  @Input() isSearch = false;
  @Input() errMsg = 'Invalid input.';

  @ViewChild('input', { static: false }) input: ElementRef;

  id: string;
  hide: boolean;
  inputControl: FormControl;
  innerType: string = InputType.TEXT;
  valueChangesSubstription: Subscription;
  propagateTouch: () => {};

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  constructor(public elementRef: ElementRef,
              @Self() private ngCrtl: NgControl) { }

  ngOnInit() {
    this.id = [this.name, getNewId().toString()].join('-');
    this.ngCrtl.valueAccessor = this;
    this.inputControl = new FormControl('', this.ngCrtl.validator);
    this.innerType = this.type;

    this.inputControl.valueChanges.subscribe(x => console.log(x));
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }

  onBlur($event?: Event) {
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

  writeValue(value: any) {
    if (value) {
      this.inputControl.setValue(value, { emitEvent: false});
    }
  }

  registerOnChange(fn) {
    this.valueChangesSubstription =
      this.inputControl.valueChanges.subscribe(fn);
  }

  registerOnTouched(fn) {
    this.propagateTouch = fn;
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
