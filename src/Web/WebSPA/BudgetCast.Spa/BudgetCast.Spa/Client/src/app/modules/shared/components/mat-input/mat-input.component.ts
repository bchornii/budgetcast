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
  Output,
  Optional
} from '@angular/core';
import { InputType } from '../input/input.component';
import { NgControl } from '@angular/forms';
import { MatFormElement } from '../mat-form-element';
import { MatInput } from '@angular/material/input';

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
  @Input() isCurrency = false;
  @Input() isTextArea = false;

  @ViewChild('input') input: ElementRef;
  @ViewChild('input', { read: MatInput }) inputDir: MatInput;

  hide: boolean;
  innerType: string = InputType.TEXT;

  @Output('blur') onBlurChange = new EventEmitter<Event>();
  @Output('focus') onFocusChange = new EventEmitter<Event>();

  get hasError(): boolean {
    return this.inputDir && this.inputDir.errorState;
  }

  constructor(public elementRef: ElementRef,
              @Optional() @Self() public ngCrtl: NgControl) {
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
