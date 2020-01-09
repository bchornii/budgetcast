import {
  Component,
  OnInit,
  Input,
  ElementRef,
  AfterViewInit,
  Self,
  OnDestroy,
  EventEmitter,
  Output,
  Optional,
  ViewChild
} from '@angular/core';
import { 
  isArray, 
  isNull, 
  isString, 
  isObject, 
  isUndefined, 
  KeyValuePair 
} from 'src/app/util/util';

import { MatFormElement } from '../mat-form-element';
import { NgControl } from '@angular/forms';

@Component({
  selector: 'app-mat-select',
  templateUrl: './mat-select.component.html',
  styles: []
})
export class MatSelectComponent extends MatFormElement implements OnInit, OnDestroy, AfterViewInit {

  @Input() label: string;
  @Input() errMsg = 'Invalid input.';
  @Input() isMultiple = false;
  @Input() isDisabled = false;
  @Input() options = [];
  @Input() hasDefault = false;
  @Input() defaultOption = 'None';
  @Input() asValue = false;

  @ViewChild('input', { static: false }) input: ElementRef;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();
  
  innerOptions: KeyValuePair[];

  constructor(public elementRef: ElementRef,
              @Optional() @Self() public ngCrtl: NgControl) {
    super(ngCrtl);
  }

  ngOnInit() {
    if (!isArray(this.options) || 
        isNull(this.options) || 
        isUndefined(this.options)) {
      this.innerOptions = [];
    }

    if (this.options.some(o => isString(o))) {
      this.innerOptions = this.options.map(
        (v: string, i: number) => new KeyValuePair(i, v));
    }

    if (this.options.some(o => isObject(o))) {
      this.innerOptions = this.options;
    }
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }
}