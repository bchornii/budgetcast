import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  Optional,
  Self,
  computed,
  effect,
  inject,
  input,
  output,
  signal,
  viewChild,
} from '@angular/core';
import { FormsModule, NgControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatError, MatFormField, MatInput, MatLabel, MatSuffix } from '@angular/material/input';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MatFormElement } from '../mat-form-element';

@Component({
  selector: 'app-mat-input',
  templateUrl: './mat-input.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatFormField,
    MatLabel,
    MatError,
    MatIcon,
    MatInput,
    MatSuffix,
    MatButtonModule,
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    CurrencyMaskModule,
  ],
})
export class MatInputComponent extends MatFormElement implements OnDestroy, AfterViewInit {
  inputName = input<string>(this.name || 'mat-input');
  label = input<string>();
  readonly = input<boolean>(false);
  type = input<string>(InputType.TEXT);
  isSearch = input<boolean>(false);
  errMsg = input<string>('Invalid input.');
  isCurrency = input<boolean>(false);
  isTextArea = input<boolean>(false);

  /** Event emitted on input blur */
  onBlurEvent = output<Event>();

  /** Event emitted on input focus */
  onFocusEvent = output<Event>();

  input = viewChild<ElementRef<HTMLInputElement>>('input');
  inputDir = viewChild('input', { read: MatInput });

  hide = signal<boolean>(false);
  innerType = signal<string>(InputType.TEXT);

  hasError = computed(() => this.inputDir()?.errorState ?? false);

  public elementRef = inject(ElementRef);

  // eslint-disable-next-line @angular-eslint/prefer-inject
  constructor(@Optional() @Self() public ngControl: NgControl) {
    // Initialize parent class with NgControl
    super(ngControl);

    // Effect to initialize innerType based on type input
    effect(() => {
      this.innerType.set(this.type());
    });

    // Effect to set element ID when name changes
    effect(() => {
      this.inputName(); // Track the signal
      this.setElementId();
    });
  }

  ngOnDestroy() {
    this.valueChangesSubscription?.unsubscribe();
  }

  ngAfterViewInit() {
    this.elementRef.nativeElement.focus = () => {
      const inputElement = this.input();
      if (inputElement) {
        inputElement.nativeElement.focus();
      }
    };
  }

  override onBlur($event?: Event) {
    super.onBlur($event);
    if ($event) {
      this.onBlurEvent.emit($event);
    }
  }

  override onFocus($event: Event) {
    super.onFocus($event);
    this.onFocusEvent.emit($event);
  }

  showPassword($event: MouseEvent) {
    $event.stopPropagation();
    $event.preventDefault();

    this.hide.update((value) => !value);
    this.innerType.update((currentType) => (currentType === 'text' ? 'password' : 'text'));
  }

  override setElementId() {
    const newId = [this.inputName(), this.getNewId().toString()].join('-');
    this.id = newId;
  }

  private getNewId(): number {
    return Math.floor(Math.random() * 1000000);
  }
}

export enum InputType {
  TEXT = 'text',
  PASSWORD = 'password',
  SEARCH = 'search',
}
