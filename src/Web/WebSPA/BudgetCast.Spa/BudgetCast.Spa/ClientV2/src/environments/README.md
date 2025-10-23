# Environment Configuration

This project uses Angular's environment configuration to manage different settings for development and production environments.

## Environment Files

The following environment files are available:

- `src/environments/environment.ts` - Development environment (default)
- `src/environments/environment.prod.ts` - Production environment
- `src/environments/environment.interface.ts` - TypeScript interface for type safety

## Configuration Properties

Each environment file contains the following configuration:

```typescript
{
  production: boolean;           // Whether this is a production build
  name: string;                 // Environment name for identification
  apiUrl: string;               // Base API endpoint URL
  baseUrl: string;              // Application base URL
  enableLogging: boolean;       // Enable/disable logging
  enableDebugMode: boolean;     // Enable/disable debug mode
  features: {                   // Feature flags
    enableBetaFeatures: boolean;
    enableAnalytics: boolean;
    enablePushNotifications: boolean;
  };
  auth: {                       // Authentication settings
    tokenExpirationTime: number;
    refreshTokenExpirationTime: number;
  };
  external: {                   // External service keys
    googleMapsApiKey: string;
    stripePublishableKey: string;
  };
}
```

## Building for Different Environments

### Development (default)
```bash
ng build
ng serve
```

### Production
```bash
ng build --configuration=production
ng serve --configuration=production
```

## Using Environment Configuration in Code

### Direct Import (Simple Usage)
```typescript
import { environment } from '../environments/environment';

export class MyService {
  private apiUrl = environment.apiUrl;
  
  getData() {
    return this.http.get(`${environment.apiUrl}/data`);
  }
}
```

### Using EnvironmentService (Recommended)
```typescript
import { EnvironmentService } from './core/services/environment.service';

export class MyService {
  constructor(private envService: EnvironmentService) {}
  
  getData() {
    const url = this.envService.buildApiUrl('data');
    return this.http.get(url);
  }
  
  isFeatureEnabled() {
    return this.envService.isFeatureEnabled('enableBetaFeatures');
  }
}
```

## Environment Service Methods

The `EnvironmentService` provides convenient methods to access configuration:

- `envService.production` - Check if production mode
- `envService.apiUrl` - Get API base URL
- `envService.buildApiUrl(endpoint)` - Build complete API URL
- `envService.buildAppUrl(path)` - Build complete app URL
- `envService.isFeatureEnabled(feature)` - Check feature flags
- `envService.getEnvironment()` - Get complete environment object

## Adding New Configuration

1. Add the new property to `environment.interface.ts`
2. Add the property to all environment files (`environment.ts`, `environment.prod.ts`)
3. Add getter method to `EnvironmentService` if needed
4. Update this documentation

## Security Notes

- Never commit sensitive keys in environment files
- Use environment variables or secure vaults for production secrets
- The `external` section is for public keys only (like Google Maps API key)
- For sensitive data, consider using Angular's `APP_INITIALIZER` with secure configuration loading

## Example Usage

See `src/app/shared/components/environment-demo.component.ts` for a complete example of how to use environment configuration in a component.