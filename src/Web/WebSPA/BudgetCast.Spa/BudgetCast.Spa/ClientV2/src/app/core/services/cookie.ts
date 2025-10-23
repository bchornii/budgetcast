import { Injectable, inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

/**
 * Modern cookie service wrapper that provides a clean interface for cookie operations
 * Uses ngx-cookie-service under the hood with additional convenience methods
 */
@Injectable({
  providedIn: 'root',
})
export class Cookie {
  private cookieService = inject(CookieService);

  /**
   * Set a cookie with optional configuration
   */
  set(
    name: string,
    value: string,
    options?: {
      expires?: number | Date;
      path?: string;
      domain?: string;
      secure?: boolean;
      sameSite?: 'Lax' | 'None' | 'Strict';
      partitioned?: boolean;
    },
  ): void {
    if (options) {
      this.cookieService.set(name, value, options);
    } else {
      this.cookieService.set(name, value);
    }
  }

  /**
   * Get a cookie value by name
   */
  get(name: string): string {
    return this.cookieService.get(name);
  }

  /**
   * Get all cookies as a key-value object
   */
  getAll(): Record<string, string> {
    return this.cookieService.getAll();
  }

  /**
   * Check if a cookie exists
   */
  exists(name: string): boolean {
    return this.cookieService.check(name);
  }

  /**
   * Delete a specific cookie
   */
  delete(
    name: string,
    path?: string,
    domain?: string,
    secure?: boolean,
    sameSite?: 'Lax' | 'None' | 'Strict',
  ): void {
    this.cookieService.delete(name, path, domain, secure, sameSite);
  }

  /**
   * Delete all cookies
   */
  deleteAll(
    path?: string,
    domain?: string,
    secure?: boolean,
    sameSite?: 'Lax' | 'None' | 'Strict',
  ): void {
    this.cookieService.deleteAll(path, domain, secure, sameSite);
  }

  /**
   * Set a secure cookie with authentication token
   * Uses secure defaults for authentication cookies
   */
  setAuthToken(token: string, expirationDays = 7): void {
    const expires = new Date();
    expires.setDate(expires.getDate() + expirationDays);

    this.set('auth_token', token, {
      expires,
      path: '/',
      secure: true,
      sameSite: 'Strict',
    });
  }

  /**
   * Get authentication token from cookies
   */
  getAuthToken(): string | null {
    const token = this.get('auth_token');
    return token || null;
  }

  /**
   * Remove authentication token
   */
  clearAuthToken(): void {
    this.delete('auth_token', '/');
  }

  /**
   * Set user preferences with JSON serialization
   */
  setUserPreferences(preferences: Record<string, unknown>): void {
    const preferencesJson = JSON.stringify(preferences);
    const expires = new Date();
    expires.setFullYear(expires.getFullYear() + 1); // 1 year expiration

    this.set('user_preferences', preferencesJson, {
      expires,
      path: '/',
      sameSite: 'Lax',
    });
  }

  /**
   * Get user preferences with JSON deserialization
   */
  getUserPreferences<T = Record<string, unknown>>(): T | null {
    const preferencesJson = this.get('user_preferences');
    if (!preferencesJson) {
      return null;
    }

    try {
      return JSON.parse(preferencesJson) as T;
    } catch {
      // If JSON parsing fails, clear the corrupted cookie
      this.delete('user_preferences', '/');
      return null;
    }
  }

  /**
   * Clear user preferences
   */
  clearUserPreferences(): void {
    this.delete('user_preferences', '/');
  }
}
