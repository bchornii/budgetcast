import { Component, OnInit, Input, Output, EventEmitter, ElementRef, AfterViewInit, ViewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormElement } from '../form-element';

export enum InputType {
  TEXT = 'text',
  PASSWORD = 'password',
  SEARCH = 'search'
}

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: InputComponent,
    multi: true
  }]
})
export class InputComponent extends FormElement implements OnInit, AfterViewInit {
  @Input() name = 'item';
  @Input() title: string;
  @Input() disabled = false;
  @Input() readonly = false;
  @Input() type = InputType.TEXT;
  @Input() isSearch = false;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  @ViewChild('input') input: ElementRef;

  defaultValue = '';
  innerType: string = InputType.TEXT;

  constructor(public elementRef: ElementRef) {
    super(elementRef);
  }

  ngOnInit() {
    this.innerType = this.type;
    this.setElementId();
  }

  ngAfterViewInit() {
    this.registerInputEvents(this.input.nativeElement);

    // Define focus method for component, because custom element without tabindex doesn`t support it.
    this.elementRef.nativeElement.focus = () => {
    this.input.nativeElement.focus();
    };
  }

  showPassword() {
    this.innerType === 'text' ? this.innerType = 'password' : this.innerType = 'text';
  }
}
