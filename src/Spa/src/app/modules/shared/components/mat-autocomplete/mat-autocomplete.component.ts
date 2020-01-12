import {
  Component,
  OnInit,
  ElementRef,
  Self,
  Input,
  AfterViewInit,
  ViewChild,
  EventEmitter,
  Output,
  OnDestroy,
  OnChanges,
  Optional
} from '@angular/core';
import { MatFormElement } from '../mat-form-element';
import { NgControl } from '@angular/forms';
import { debounceTime, tap, distinctUntilChanged, filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-mat-autocomplete',
  templateUrl: './mat-autocomplete.component.html'
})
export class MatAutocompleteComponent extends MatFormElement implements OnInit, OnDestroy, AfterViewInit {
  @Input() name = 'item';
  @Input() label: string;
  @Input() readonly = false;
  @Input() errMsg = 'Invalid input.';
  @Input() options: any[];
  @Input() debounceTime = 300;
  @Input() isLoading = false;

  @ViewChild('input', { static: false }) input: ElementRef;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  constructor(public elementRef: ElementRef,
              @Optional() @Self() public ngCrtl: NgControl) {
      super(ngCrtl);
  }

  ngOnInit() {
    this.setElementId();
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      this.input.nativeElement.focus();
    };
  }

  registerOnChange(fn) {
    this.valueChangesSubstription =
      this.inputControl.valueChanges.pipe(
        debounceTime(this.debounceTime),
        map(emittedValue => emittedValue.trim()),
        distinctUntilChanged()
      ).subscribe(fn);
  }

}
