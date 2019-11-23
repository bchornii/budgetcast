import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { RouterModule, Router } from '@angular/router';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule, ToastrService } from 'ngx-toastr';

import { AppComponent } from './app.component';
import { checkIfUserIsAuthenticated } from './account/login/check-login-initializer';
import { AccountService } from './account/account.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { HttpConfigInterceptor } from './shared/services/http-config.interceptor';
import { AppBootstrapModule } from './shared/app-bootstrap.module';
import { NavComponent } from './nav/nav.component';
import { appRoutes } from './app.routes';
import { HomeComponent } from './home/home.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),

    AppBootstrapModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      timeOut: 10000,
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
      closeButton: true
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpConfigInterceptor,
      multi: true,
      deps: [AccountService, Router, ToastrService]
    }
    ,
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
