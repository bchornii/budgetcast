import {
  Component,
  OnInit,
  Input,
  ViewChild,
  ElementRef,
  AfterViewInit,
  Self,
  OnDestroy,
  EventEmitter,
  Output
} from '@angular/core';
import { InputType } from '../input/input.component';
import { NgControl } from '@angular/forms';
import { MatFormElement } from '../mat-form-element';

@Component({
  selector: 'app-mat-input',
  templateUrl: './mat-input.component.html'
})
export class MatInputComponent extends MatFormElement implements OnInit, OnDestroy, AfterViewInit {
  @Input() name = 'item';
  @Input() label: string;
  @Input() readonly = false;
  @Input() type: string = InputType.TEXT;
  @Input() isSearch = false;
  @Input() errMsg = 'Invalid input.';

  @ViewChild('input', { static: false }) input: ElementRef;

  hide: boolean;
  innerType: string = InputType.TEXT;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  constructor(public elementRef: ElementRef,
              @Self() public ngCrtl: NgControl) {
    super(ngCrtl);
  }

  ngOnInit() {
    this.setElementId();
    this.innerType = this.type;
  }

  ngOnDestroy() {
    this.valueChangesSubstription.unsubscribe();
  }

  ngAfterViewInit() {
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
