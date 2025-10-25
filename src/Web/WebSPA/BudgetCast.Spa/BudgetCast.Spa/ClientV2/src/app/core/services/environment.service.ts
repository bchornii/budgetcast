import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Environment } from '../../../environments/environment.interface';

@Injectable({
  providedIn: 'root',
})
export class EnvironmentService {
  private readonly env: Environment = environment;

  get production(): boolean {
    return this.env.production;
  }

  get name(): string {
    return this.env.name;
  }

  get apiUrl(): string {
    return this.env.apiUrl;
  }

  get baseUrl(): string {
    return this.env.baseUrl;
  }

  get devBaseUrl(): string {
    return this.env.devBaseUrl;
  }

  get enableLogging(): boolean {
    return this.env.enableLogging;
  }

  get enableDebugMode(): boolean {
    return this.env.enableDebugMode;
  }

  get features() {
    return this.env.features;
  }

  get auth() {
    return this.env.auth;
  }

  get external() {
    return this.env.external;
  }

  /**
   * Check if a specific feature is enabled
   */
  isFeatureEnabled(feature: keyof Environment['features']): boolean {
    return this.env.features[feature];
  }

  /**
   * Get the full environment configuration
   */
  getEnvironment(): Environment {
    return this.env;
  }

  /**
   * Build a complete API endpoint URL
   */
  buildApiUrl(endpoint: string): string {
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;
    return `${this.env.apiUrl}${cleanEndpoint}`;
  }

  /**
   * Build a complete application URL
   */
  buildAppUrl(path: string): string {
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    return `${this.env.baseUrl}${cleanPath}`;
  }
}
