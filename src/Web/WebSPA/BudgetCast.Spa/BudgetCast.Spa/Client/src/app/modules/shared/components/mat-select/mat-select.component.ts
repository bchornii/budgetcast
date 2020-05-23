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
  @Input() hasDefault = false;  
  @Input() asValue = false;
  @Input() set defaultOption(value) {
    if(!value) {
      this.innerDefaultOption = new KeyValuePair(0, 'None');
    } else {
      this.innerDefaultOption = value;
    }
  };
  @Input() set options(value) {
    if (!isArray(value) || 
        isNull(value) || 
        isUndefined(value)) {
      this.innerOptions = [];
      return;
    }

    if (value.some(o => isString(o))) {
      this.innerOptions = value.map(
        (v: string, i: number) => new KeyValuePair(i, v));
      return;
    }

    if (value.some(o => isObject(o))) {
      this.innerOptions = value;
    }
  };
  @Output() onSelection = new EventEmitter<any>();

  @ViewChild('input', { static: false }) input: ElementRef;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();
  
  innerDefaultOption: KeyValuePair;
  innerOptions: KeyValuePair[];

  constructor(public elementRef: ElementRef,
              @Optional() @Self() public ngCrtl: NgControl) {
    super(ngCrtl);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }

  onSelect($event) {
    this.onSelection.emit($event.value);    
  }
}