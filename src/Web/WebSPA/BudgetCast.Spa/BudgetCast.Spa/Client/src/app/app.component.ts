import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from './services/auth.service';
import { NotificationsService } from './services/notifications-hub.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {

  private authSubscription: Subscription;
  
  constructor(private authService: AuthService,
              private notificationService: NotificationsService) {}

  ngOnInit() {
    this.authSubscription = this.authService.userIdentity$
      .subscribe(async(a) => {
        if(a.isAuthenticated) {
          await this.notificationService
            .initializeConnection()
            .addListeners()
            .startCommunication();
        } else {
          await this.notificationService
            .stopCommunication();
        }
      });
  }

  async ngOnDestroy() {
    this.authSubscription.unsubscribe();

    // destroy signalr connection on component destroy
    // in order to avoid connection leaks and increases
    await this.notificationService
      .stopCommunication();
  }
}
