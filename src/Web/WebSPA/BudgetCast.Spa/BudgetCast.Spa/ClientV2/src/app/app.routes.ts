import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'welcome',
    loadChildren: () =>
      import('./core/auth/pages/welcome/welcome.routes').then((m) => m.welcomeRoutes),
  },
  {
    path: '**',
    redirectTo: '/welcome',
    pathMatch: 'full',
  },
];
