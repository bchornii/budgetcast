import { Environment } from './environment.interface';

export const environment: Environment = {
  production: true,
  name: 'production',
  apiUrl: 'https://api.eoi-portal.gov.ab.ca/api',
  baseUrl: 'https://eoi-portal.gov.ab.ca',
  enableLogging: false,
  enableDebugMode: false,
  features: {
    enableBetaFeatures: false,
    enableAnalytics: true,
    enablePushNotifications: true,
  },
  auth: {
    tokenExpirationTime: 1800000, // 30 minutes in milliseconds
    refreshTokenExpirationTime: 43200000, // 12 hours in milliseconds
  },
  external: {
    googleMapsApiKey: 'prod-google-maps-key',
    stripePublishableKey: 'pk_live_...',
  },
};
