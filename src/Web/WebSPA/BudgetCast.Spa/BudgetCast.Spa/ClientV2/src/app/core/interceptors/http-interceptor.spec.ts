import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { httpInterceptor } from './http-interceptor';

describe('httpInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([httpInterceptor])),
        provideHttpClientTesting(),
        { provide: Router, useValue: routerSpy },
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should add default headers to requests', () => {
    const testUrl = '/test';

    httpClient.get(testUrl).subscribe();

    const req = httpTestingController.expectOne(testUrl);
    expect(req.request.headers.get('Content-Type')).toBe('application/json');
    expect(req.request.headers.get('X-Requested-With')).toBe('XMLHttpRequest');

    req.flush({});
  });

  it('should handle 401 authentication errors', () => {
    const testUrl = '/test';
    const consoleSpy = spyOn(console, 'warn');

    httpClient.get(testUrl).subscribe({
      error: (error) => {
        expect(error.status).toBe(401);
      },
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush({}, { status: 401, statusText: 'Unauthorized' });

    expect(consoleSpy).toHaveBeenCalledWith('Authentication error detected:', 401);
  });

  it('should handle 403 forbidden errors', () => {
    const testUrl = '/test';
    const consoleSpy = spyOn(console, 'warn');

    httpClient.get(testUrl).subscribe({
      error: (error) => {
        expect(error.status).toBe(403);
      },
    });

    const req = httpTestingController.expectOne(testUrl);
    req.flush({}, { status: 403, statusText: 'Forbidden' });

    expect(consoleSpy).toHaveBeenCalledWith('Authentication error detected:', 403);
  });
});
