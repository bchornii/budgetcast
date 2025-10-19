import { 
  Component, 
  OnInit, 
  Optional, 
  Self, 
  Input, 
  OnDestroy, 
  AfterViewInit, 
  ViewChild, 
  ElementRef, 
  Output, 
  EventEmitter} from '@angular/core';
import { MatFormElement } from '../mat-form-element';
import { NgControl } from '@angular/forms';

import { parse } from 'date-fns';

import { SMALL_WIDTH_BREAKPOINT } from 'src/app/util/constants/response-status';

@Component({
  selector: 'app-mat-datepicker',
  templateUrl: './mat-datepicker.component.html'
})
export class MatDatepickerComponent extends MatFormElement implements OnInit, OnDestroy, AfterViewInit {

  private mediaMatcher: MediaQueryList =
    matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);

  @Input() name = 'item';
  @Input() label: string;
  @Input() errMsg = 'Invalid input.';
  @Input() format = "MM-DD-YYYY";
  @Input() minDate = "01-01-2000";
  @Input() maxDate = "01-01-2030";

  @ViewChild('input') input: ElementRef;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();
  
  minDt: Date;
  maxDt: Date;

  constructor(public elementRef: ElementRef,
    @Optional() @Self() public ngCrtl: NgControl) {
    super(ngCrtl);        
  }

  ngOnInit() {
    this.setElementId();
  // parse strings like "MM-DD-YYYY" into Date objects
  this.minDt = parse(this.minDate, this.format, new Date());
  this.maxDt = parse(this.maxDate, this.format, new Date());
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
