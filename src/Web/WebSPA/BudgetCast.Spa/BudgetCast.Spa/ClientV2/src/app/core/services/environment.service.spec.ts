import { TestBed } from '@angular/core/testing';
import { EnvironmentService } from './environment.service';

describe('EnvironmentService', () => {
  let service: EnvironmentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EnvironmentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return production status', () => {
    expect(typeof service.production).toBe('boolean');
  });

  it('should return environment name', () => {
    expect(typeof service.name).toBe('string');
    expect(service.name).toBeTruthy();
  });

  it('should return API URL', () => {
    expect(typeof service.apiUrl).toBe('string');
    expect(service.apiUrl).toBeTruthy();
  });

  it('should build API URL correctly', () => {
    const endpoint = 'users';
    const fullUrl = service.buildApiUrl(endpoint);
    expect(fullUrl).toBe(`${service.apiUrl}/${endpoint}`);
  });

  it('should build API URL with leading slash correctly', () => {
    const endpoint = '/users';
    const fullUrl = service.buildApiUrl(endpoint);
    expect(fullUrl).toBe(`${service.apiUrl}/users`);
  });

  it('should build app URL correctly', () => {
    const path = 'dashboard';
    const fullUrl = service.buildAppUrl(path);
    expect(fullUrl).toBe(`${service.baseUrl}/${path}`);
  });

  it('should check feature flags correctly', () => {
    const isAnalyticsEnabled = service.isFeatureEnabled('enableAnalytics');
    expect(typeof isAnalyticsEnabled).toBe('boolean');
  });

  it('should return complete environment', () => {
    const env = service.getEnvironment();
    expect(env).toBeTruthy();
    expect(env.production).toBeDefined();
    expect(env.apiUrl).toBeDefined();
    expect(env.features).toBeDefined();
  });
});
