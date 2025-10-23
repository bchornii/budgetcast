import { TestBed } from '@angular/core/testing';
import { CookieService } from 'ngx-cookie-service';
import { Cookie } from './cookie';

describe('Cookie', () => {
  let service: Cookie;
  let cookieServiceSpy: jasmine.SpyObj<CookieService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('CookieService', {
      set: undefined,
      get: '',
      getAll: {},
      check: false,
      delete: undefined,
      deleteAll: undefined,
    });

    TestBed.configureTestingModule({
      providers: [Cookie, { provide: CookieService, useValue: spy }],
    });

    service = TestBed.inject(Cookie);
    cookieServiceSpy = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('set', () => {
    it('should call cookieService.set with options object', () => {
      const name = 'test-cookie';
      const value = 'test-value';
      const options = {
        expires: new Date(),
        path: '/',
        domain: 'example.com',
        secure: true,
        sameSite: 'Strict' as const,
      };

      service.set(name, value, options);

      expect(cookieServiceSpy.set).toHaveBeenCalledWith(name, value, options);
    });

    it('should call cookieService.set with minimal parameters', () => {
      const name = 'test-cookie';
      const value = 'test-value';

      service.set(name, value);

      expect(cookieServiceSpy.set).toHaveBeenCalledWith(name, value);
    });
  });

  describe('get', () => {
    it('should return cookie value', () => {
      const name = 'test-cookie';
      const expectedValue = 'test-value';
      cookieServiceSpy.get.and.returnValue(expectedValue);

      const result = service.get(name);

      expect(result).toBe(expectedValue);
      expect(cookieServiceSpy.get).toHaveBeenCalledWith(name);
    });
  });

  describe('getAll', () => {
    it('should return all cookies', () => {
      const expectedCookies = { cookie1: 'value1', cookie2: 'value2' };
      cookieServiceSpy.getAll.and.returnValue(expectedCookies);

      const result = service.getAll();

      expect(result).toBe(expectedCookies);
      expect(cookieServiceSpy.getAll).toHaveBeenCalled();
    });
  });

  describe('exists', () => {
    it('should return true when cookie exists', () => {
      const name = 'test-cookie';
      cookieServiceSpy.check.and.returnValue(true);

      const result = service.exists(name);

      expect(result).toBe(true);
      expect(cookieServiceSpy.check).toHaveBeenCalledWith(name);
    });

    it('should return false when cookie does not exist', () => {
      const name = 'test-cookie';
      cookieServiceSpy.check.and.returnValue(false);

      const result = service.exists(name);

      expect(result).toBe(false);
      expect(cookieServiceSpy.check).toHaveBeenCalledWith(name);
    });
  });

  describe('setAuthToken', () => {
    it('should set auth token with secure defaults', () => {
      const token = 'test-token';
      const expirationDays = 7;

      service.setAuthToken(token, expirationDays);

      expect(cookieServiceSpy.set).toHaveBeenCalledWith('auth_token', token, {
        expires: jasmine.any(Date),
        path: '/',
        secure: true,
        sameSite: 'Strict',
      });
    });

    it('should set auth token with default expiration', () => {
      const token = 'test-token';

      service.setAuthToken(token);

      expect(cookieServiceSpy.set).toHaveBeenCalledWith('auth_token', token, {
        expires: jasmine.any(Date),
        path: '/',
        secure: true,
        sameSite: 'Strict',
      });
    });
  });

  describe('getAuthToken', () => {
    it('should return auth token when exists', () => {
      const expectedToken = 'test-token';
      cookieServiceSpy.get.and.returnValue(expectedToken);

      const result = service.getAuthToken();

      expect(result).toBe(expectedToken);
      expect(cookieServiceSpy.get).toHaveBeenCalledWith('auth_token');
    });

    it('should return null when token does not exist', () => {
      cookieServiceSpy.get.and.returnValue('');

      const result = service.getAuthToken();

      expect(result).toBeNull();
      expect(cookieServiceSpy.get).toHaveBeenCalledWith('auth_token');
    });
  });

  describe('setUserPreferences', () => {
    it('should set user preferences as JSON', () => {
      const preferences = { theme: 'dark', language: 'en' };
      const expectedJson = JSON.stringify(preferences);

      service.setUserPreferences(preferences);

      expect(cookieServiceSpy.set).toHaveBeenCalledWith('user_preferences', expectedJson, {
        expires: jasmine.any(Date),
        path: '/',
        sameSite: 'Lax',
      });
    });
  });

  describe('getUserPreferences', () => {
    it('should return parsed user preferences', () => {
      const preferences = { theme: 'dark', language: 'en' };
      const preferencesJson = JSON.stringify(preferences);
      cookieServiceSpy.get.and.returnValue(preferencesJson);

      const result = service.getUserPreferences();

      expect(result).toEqual(preferences);
      expect(cookieServiceSpy.get).toHaveBeenCalledWith('user_preferences');
    });

    it('should return null when preferences do not exist', () => {
      cookieServiceSpy.get.and.returnValue('');

      const result = service.getUserPreferences();

      expect(result).toBeNull();
    });

    it('should handle invalid JSON and clear corrupted cookie', () => {
      cookieServiceSpy.get.and.returnValue('invalid-json');

      const result = service.getUserPreferences();

      expect(result).toBeNull();
      expect(cookieServiceSpy.delete).toHaveBeenCalledWith(
        'user_preferences',
        '/',
        undefined,
        undefined,
        undefined,
      );
    });
  });
});
