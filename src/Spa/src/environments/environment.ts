// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

const apiUrl = 'http://localhost:8200/api';

const accountApi = {
  isAuthenticated: `${apiUrl}/account/isAuthenticated`,
  signInWithGoogle: `${apiUrl}/account/signInWithGoogle`,
  signInWithFacebook: `${apiUrl}/account/signInWithFacebook`,
  login: `${apiUrl}/account/login`,
  logout: `${apiUrl}/account/logout`,
  check: `${apiUrl}/account/check`,
  updateProfile: `${apiUrl}/account/updateProfile`,
  register: `${apiUrl}/account/register`,
  forgotPassword: `${apiUrl}/account/forgotPassword`,
  resetPassword: `${apiUrl}/account/resetPassword`
};

const recipesApi = {
  categories: `${apiUrl}/recipes/categories`
};

const api = {
  accountApi,
  recipesApi
};

export const environment = {
  production: false,
  api
};
