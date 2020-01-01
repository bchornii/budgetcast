import { Subscription } from 'rxjs';
import { OnInit, Output, EventEmitter, ElementRef, Injector, Directive } from '@angular/core';
import { NgForm } from '@angular/forms';

@Directive({
	selector: 'form[appForm]'
})
export class FormDirective implements OnInit {

    @Output() appSubmit = new EventEmitter();

    ngForm: NgForm;
    submitSub: Subscription;

    constructor(private elementRef: ElementRef,
                private injector: Injector) {
    }

    ngOnInit(){
        const ngForm = this.injector.get(NgForm);

        this.submitSub = ngForm.ngSubmit.subscribe(($event) => {
          if (ngForm.valid) {
            this.appSubmit.emit($event);
          } else {
            Object.keys(ngForm.controls).forEach(field => {
              const control = ngForm.controls[field];
              control.markAsTouched({ onlySelf: true });
              control.markAsDirty({ onlySelf: true });
            });

            window.setTimeout(() => {
                this.focusFirstErrorField(this.elementRef);
              }, 0);
            }
        });
    }

    private focusFirstErrorField(formElement: ElementRef): void {
      const el: HTMLElement = formElement.nativeElement;

      const element: any = el.querySelector('.ng-invalid.ng-dirty');

      if (element && element.focus) {
        element.focus();
      }
  }
}
