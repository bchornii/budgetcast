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
  Optional
} from '@angular/core';
import { MatFormElement } from '../mat-form-element';
import { NgControl } from '@angular/forms';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';

@Component({
  selector: 'app-mat-chips-autocomplete',
  templateUrl: './mat-chips-autocomplete.component.html'
})
export class MatChipsAutocompleteComponent extends MatFormElement implements OnInit, OnDestroy, AfterViewInit {
  @Input() name = 'item';
  @Input() label: string;
  @Input() readonly = false;
  @Input() removable = true;
  @Input() debounceTime = 300;
  @Input() isLoading = false;
  @Input() options: any[];
  @Input() data: any[];

  @ViewChild('input', { static: false }) input: ElementRef;
  @ViewChild('auto', {static: false}) matAutocomplete: MatAutocomplete;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  separatorKeysCodes: number[] = [ENTER, COMMA];

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

  add(event: MatChipInputEvent): void {
    if (!this.matAutocomplete.isOpen) {
      const value = event.value;
      const input = event.input;

      if ((value || '').trim()) {
        this.data.push(value.trim());
      }

      if (input) {
        input.value = '';
      }

      this.inputControl.setValue(null, { emitEvent: false});
    }
  }

  remove(item: string): void {
    const index = this.data.indexOf(item);

    if (index >= 0) {
      this.data.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.data.push(event.option.viewValue);
    this.input.nativeElement.value = '';
    this.inputControl.setValue(null, { emitEvent: false});
  }

}
