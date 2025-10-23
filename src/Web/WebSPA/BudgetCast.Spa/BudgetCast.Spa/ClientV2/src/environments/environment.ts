// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --configuration production` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { Environment } from './environment.interface';

export const environment: Environment = {
  production: false,
  name: 'development',
  apiUrl: 'http://localhost:3000/api',
  baseUrl: 'http://localhost:4200',
  enableLogging: true,
  enableDebugMode: true,
  features: {
    enableBetaFeatures: true,
    enableAnalytics: false,
    enablePushNotifications: false,
  },
  auth: {
    tokenExpirationTime: 3600000, // 1 hour in milliseconds
    refreshTokenExpirationTime: 86400000, // 24 hours in milliseconds
  },
  external: {
    googleMapsApiKey: 'dev-google-maps-key',
    stripePublishableKey: 'pk_test_...',
  },
};
