// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

const apiUrl = 'https://localhost:44305/api';

const accountApi = {
  isAuthenticated: `${apiUrl}/account/isAuthenticated`,
  signInWithGoogle: `${apiUrl}/account/signInWithGoogle`,
  logout: `${apiUrl}/account/logout`,
  check: `${apiUrl}/account/check`
};

const api = {
  accountApi
};

export const environment = {
  production: false,
  api
};
