import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, throwError, timer } from 'rxjs';
import { catchError, finalize, retry } from 'rxjs/operators';
import { EnvironmentService } from './environment.service';

export interface ApiError {
  code: string;
  message: string;
  details?: any;
  timestamp: Date;
  endpoint?: string;
}

export interface LoadingState {
  isLoading: boolean;
  operation?: string;
}

@Injectable({
  providedIn: 'root',
})
export class BaseService {
  protected readonly environmentService = inject(EnvironmentService);

  // Loading state management using signals
  private readonly _loadingState = signal<LoadingState>({ isLoading: false });
  readonly loadingState = this._loadingState.asReadonly();

  // Cache for GET requests
  private readonly cache = new Map<string, { data: any; timestamp: number; ttl: number }>();
  private readonly defaultCacheTtl = 5 * 60 * 1000; // 5 minutes

  /**
   * Execute HTTP request with loading state management
   */
  protected executeRequest<T>(
    request: Observable<T>,
    operationName?: string,
    endpoint?: string,
  ): Observable<T> {
    this.setLoadingState(true, operationName);

    return request.pipe(
      catchError((error) => this.handleError(error, endpoint)),
      finalize(() => this.setLoadingState(false, operationName)),
    );
  }

  /**
   * Set loading state for an operation
   */
  protected setLoadingState(isLoading: boolean, operation?: string): void {
    this._loadingState.set({ isLoading, operation });
  }

  /**
   * Handle HTTP errors with proper error transformation
   */
  protected handleError = (error: HttpErrorResponse, endpoint?: string): Observable<never> => {
    const apiError: ApiError = {
      code: this.getErrorCode(error),
      message: this.getErrorMessage(error),
      details: error.error,
      timestamp: new Date(),
      endpoint,
    };

    this.log('error', `API Error: ${apiError.message}`, {
      endpoint,
      status: error.status,
      error: error.error,
    });

    return throwError(() => apiError);
  };

  /**
   * Log messages based on environment settings
   */
  protected log(level: 'info' | 'warn' | 'error' | 'debug', message: string, data?: any): void {
    if (!this.environmentService.enableLogging) {
      return;
    }

    const timestamp = new Date().toISOString();
    const logMessage = `[${timestamp}] [${level.toUpperCase()}] ${message}`;

    switch (level) {
      case 'error':
        console.error('%c[BaseService]', 'background: #222; color: #e65454ff', logMessage, data);
        break;
      case 'warn':
        console.warn('%c[BaseService]', 'background: #222; color: #e6b54fff', logMessage, data);
        break;
      case 'debug':
        if (this.environmentService.enableDebugMode) {
          console.debug('%c[BaseService]', 'background: #222; color: #78e247ff', logMessage, data);
        }
        break;
      default:
        console.log('%c[BaseService]', 'background: #222; color: #78e247ff', logMessage, data);
    }
  }

  /**
   * Retry logic for failed HTTP requests - only retries on server errors (5xx)
   */
  protected retryRequest<T>(
    retryCount: number = 3,
    delayMs: number = 1000,
  ): (source: Observable<T>) => Observable<T> {
    return (source: Observable<T>) =>
      source.pipe(
        retry({
          count: retryCount,
          delay: (error: HttpErrorResponse, retryIndex: number) => {
            // Only retry server errors (5xx) and network errors (status 0)
            if (error.status === 0 || (error.status >= 500 && error.status <= 599)) {
              this.log(
                'warn',
                `Retrying request (attempt ${retryIndex}/${retryCount}) after ${delayMs}ms due to ${error.status} error`,
              );
              // Exponential backoff: delayMs * 2^retryIndex
              return timer(delayMs * Math.pow(2, retryIndex));
            } else {
              // Don't retry client errors (4xx) or other errors
              this.log('warn', `Not retrying request due to ${error.status} error (client error)`);
              return throwError(() => error);
            }
          },
        }),
      );
  }

  /**
   * Cache management for GET requests
   */
  protected setCache(key: string, data: any, ttlMs: number = this.defaultCacheTtl): void {
    this.cache.set(key, {
      data,
      timestamp: Date.now(),
      ttl: ttlMs,
    });
  }

  /**
   * Get cached data if still valid
   */
  protected getCache<T>(key: string): T | null {
    const cached = this.cache.get(key);
    if (!cached) {
      return null;
    }

    const isExpired = Date.now() - cached.timestamp > cached.ttl;
    if (isExpired) {
      this.cache.delete(key);
      return null;
    }

    this.log('debug', `Cache hit for key: ${key}`);
    return cached.data as T;
  }

  /**
   * Clear cache by key or clear all cache
   */
  protected clearCache(key?: string): void {
    if (key) {
      this.cache.delete(key);
      this.log('debug', `Cache cleared for key: ${key}`);
    } else {
      this.cache.clear();
      this.log('debug', 'All cache cleared');
    }
  }

  /**
   * Build API URL using environment service
   */
  protected buildApiUrl(endpoint: string): string {
    return this.environmentService.buildApiUrl(endpoint);
  }

  /**
   * Check if we're in production mode
   */
  protected get isProduction(): boolean {
    return this.environmentService.production;
  }

  /**
   * Check if debug mode is enabled
   */
  protected get isDebugMode(): boolean {
    return this.environmentService.enableDebugMode;
  }

  /**
   * Get user-friendly error message from HTTP error
   */
  private getErrorMessage(error: HttpErrorResponse): string {
    if (error.error?.message) {
      return error.error.message;
    }

    switch (error.status) {
      case 0:
        return 'Network error. Please check your internet connection.';
      case 400:
        return 'Invalid request. Please check your input.';
      case 401:
        return 'Authentication required. Please log in.';
      case 403:
        return 'Access denied. You do not have permission for this action.';
      case 404:
        return 'The requested resource was not found.';
      case 408:
        return 'Request timeout. Please try again.';
      case 429:
        return 'Too many requests. Please wait and try again.';
      case 500:
        return 'Server error. Please try again later.';
      case 502:
      case 503:
      case 504:
        return 'Service temporarily unavailable. Please try again later.';
      default:
        return `An unexpected error occurred (${error.status}).`;
    }
  }

  /**
   * Get error code from HTTP error
   */
  private getErrorCode(error: HttpErrorResponse): string {
    if (error.error?.code) {
      return error.error.code;
    }
    return `HTTP_${error.status}`;
  }
}
