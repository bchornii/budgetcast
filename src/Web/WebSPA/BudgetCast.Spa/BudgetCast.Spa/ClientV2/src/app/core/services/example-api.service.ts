import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { BaseService } from './base-service';

// TODO: Place models into a models folder
export interface CheckResponse {
  status: string;
  message?: string;
  timestamp?: string;
}

export interface DataResponse {
  data: any;
  count?: number;
  page?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ExampleTestService extends BaseService {
  private readonly http = inject(HttpClient);

  // Base URLs for the endpoints
  private readonly baseUrl = 'https://mp0f5848b3205858bb84.free.beeceptor.com';

  /**
   * Call the /check endpoint to verify service status
   */
  checkService(): Observable<CheckResponse> {
    const url = `${this.baseUrl}/check`;
    const cacheKey = 'service-check';

    // Check cache first (cache for 1 minute for status checks)
    const cached = this.getCache<CheckResponse>(cacheKey);
    if (cached) {
      this.log('info', 'Using cached service check result');
      return new Observable((subscriber) => {
        subscriber.next(cached);
        subscriber.complete();
      });
    }

    this.log('info', 'Making request to check endpoint');

    const request = this.http.get<CheckResponse>(url).pipe(
      tap((response) => {
        this.log('info', 'Check endpoint response received', response);
        this.setCache(cacheKey, response, 60 * 1000);
      }),
      this.retryRequest(2, 500),
    );

    return this.executeRequest(request, 'Checking service status', url);
  }

  /**
   * Call the /data endpoint to fetch data
   */
  fetchData(): Observable<DataResponse> {
    const url = `${this.baseUrl}/data`;
    const cacheKey = 'service-data';

    // Check cache first (cache for 2 minutes for data)
    const cached = this.getCache<DataResponse>(cacheKey);
    if (cached) {
      this.log('info', 'Using cached data result');
      return new Observable((subscriber) => {
        subscriber.next(cached);
        subscriber.complete();
      });
    }

    this.log('info', 'Making request to data endpoint');

    const request = this.http.get<DataResponse>(url).pipe(
      tap((response) => {
        this.log('info', 'Data endpoint response received', response);
        this.setCache(cacheKey, response, 2 * 60 * 1000);
      }),
      this.retryRequest(3, 1000),
    );

    return this.executeRequest(request, 'Fetching data', url);
  }

  /**
   * Call the /data endpoint with query parameters
   */
  fetchDataWithParams(page: number = 1, limit: number = 10): Observable<DataResponse> {
    const url = `${this.baseUrl}/data`;
    const cacheKey = `service-data-${page}-${limit}`;

    // Check cache first
    const cached = this.getCache<DataResponse>(cacheKey);
    if (cached) {
      this.log('info', `Using cached data result for page ${page}, limit ${limit}`);
      return new Observable((subscriber) => {
        subscriber.next(cached);
        subscriber.complete();
      });
    }

    this.log('info', `Making request to data endpoint with params: page=${page}, limit=${limit}`);

    const request = this.http
      .get<DataResponse>(url, {
        params: {
          page: page.toString(),
          limit: limit.toString(),
        },
      })
      .pipe(
        tap((response) => {
          this.log('info', 'Data endpoint with params response received', response);
          this.setCache(cacheKey, response, 90 * 1000);
        }),
        this.retryRequest(2, 800),
      );

    return this.executeRequest(request, `Fetching data (page ${page})`, url);
  }

  /**
   * Demonstrate POST request to check endpoint (if it accepts POST)
   */
  postCheck(payload: { message: string }): Observable<CheckResponse> {
    const url = `${this.baseUrl}/check`;

    this.log('info', 'Making POST request to check endpoint', payload);

    const request = this.http.post<CheckResponse>(url, payload).pipe(
      tap((response) => {
        this.log('info', 'POST check endpoint response received', response);
        this.clearCache('service-check');
      }),
      this.retryRequest(1, 1000),
    );

    return this.executeRequest(request, 'Posting to check endpoint', url);
  }

  /**
   * Clear all cached data for this service
   */
  clearServiceCache(): void {
    this.clearCache('service-check');
    this.clearCache('service-data');
    this.log('info', 'Cleared all service cache');
  }

  /**
   * Get current loading state - useful for components
   */
  get isLoading(): boolean {
    return this.loadingState().isLoading;
  }

  /**
   * Get current operation being performed
   */
  get currentOperation(): string | undefined {
    return this.loadingState().operation;
  }
}
