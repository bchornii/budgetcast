
const apiUrl = 'https://dashboard.api.budget-cast.com/api';

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
  production: true,
  api
};
