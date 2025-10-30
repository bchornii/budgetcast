import { DestroyRef, Directive, ElementRef, inject, output } from '@angular/core';
import { NgForm } from '@angular/forms';

@Directive({
  selector: 'form[appForm]',
})
export class FormDirective {
  appSubmit = output<Event>();

  private ngForm = inject(NgForm);
  private elementRef = inject(ElementRef);
  private destroyRef = inject(DestroyRef);

  constructor() {
    const subscription = this.ngForm.ngSubmit.subscribe(($event) => {
      if (this.ngForm.valid) {
        this.appSubmit.emit($event);
      } else {
        Object.keys(this.ngForm.controls).forEach((field) => {
          const control = this.ngForm.controls[field];
          control.markAsTouched({ onlySelf: true });
          control.markAsDirty({ onlySelf: true });
        });

        window.setTimeout(() => {
          this.focusFirstErrorField(this.elementRef);
        }, 0);
      }
    });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  private focusFirstErrorField(formElement: ElementRef): void {
    const el: HTMLElement = formElement.nativeElement;
    const element: any = el.querySelector('.ng-invalid.ng-dirty');

    if (element && element.focus) {
      element.focus();
    }
  }
}
