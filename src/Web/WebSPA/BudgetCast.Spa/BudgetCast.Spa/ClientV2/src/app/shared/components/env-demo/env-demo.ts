import { Component, OnInit, inject } from '@angular/core';
import { EnvironmentService } from '../../../core/services/environment.service';

@Component({
  selector: 'app-env-demo',
  imports: [],
  templateUrl: './env-demo.html',
  styleUrl: './env-demo.scss',
})
export class EnvDemo implements OnInit {
  exampleApiUrl = '';

  environmentService = inject(EnvironmentService);

  ngOnInit(): void {
    this.exampleApiUrl = this.environmentService.buildApiUrl('users/profile');

    if (this.environmentService.enableDebugMode) {
      console.log('Debug mode is enabled');
      console.log('Current environment:', this.environmentService.getEnvironment());
    }

    if (this.environmentService.isFeatureEnabled('enableAnalytics')) {
      console.log('Analytics is enabled for this environment');
    }
  }
}
