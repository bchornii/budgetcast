import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';

import { AppComponent } from './app.component';
import { AccountComponent } from './account/account.component';
import { checkIfUserIsAuthenticated } from './account/check-login-initializer';
import { AccountService } from './account/account.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { HttpConfigInterceptor } from './common/http-config.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    AccountComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpConfigInterceptor,
      multi: true,
      deps: [AccountService]
    },
    {
      provide: APP_INITIALIZER,
      useFactory: checkIfUserIsAuthenticated,
      multi: true,
      deps: [AccountService]
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
