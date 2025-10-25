export interface Environment {
  production: boolean;
  name: string;
  apiUrl: string;
  baseUrl: string;
  devBaseUrl: string;
  enableLogging: boolean;
  enableDebugMode: boolean;
  features: {
    enableBetaFeatures: boolean;
    enableAnalytics: boolean;
    enablePushNotifications: boolean;
  };
  auth: {
    tokenExpirationTime: number;
    refreshTokenExpirationTime: number;
  };
  external: {
    googleMapsApiKey: string;
    stripePublishableKey: string;
  };
}
