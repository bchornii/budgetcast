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

import * as moment from 'moment';
import {MAT_MOMENT_DATE_FORMATS, MomentDateAdapter} from '@angular/material-moment-adapter';
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from '@angular/material/core';

import { SMALL_WIDTH_BREAKPOINT } from 'src/app/util/constants/response-status';
import { Moment } from 'moment';

@Component({
  selector: 'app-mat-datepicker',
  templateUrl: './mat-datepicker.component.html',
  providers: [    
    {provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE]},
    {provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS},
  ]
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

  @ViewChild('input', { static: false }) input: ElementRef;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();
  
  minDt: Moment;
  maxDt: Moment;

  constructor(public elementRef: ElementRef,
    @Optional() @Self() public ngCrtl: NgControl) {
    super(ngCrtl);        
  }

  ngOnInit() {
    this.setElementId();
    this.minDt = moment(this.minDate, this.format);
    this.maxDt = moment(this.maxDate, this.format);
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
