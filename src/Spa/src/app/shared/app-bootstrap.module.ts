import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NgbModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  imports: [
    CommonModule,

    NgbDropdownModule
  ],
  exports: [NgbDropdownModule]
})
export class AppBootstrapModule {}
