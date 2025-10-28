import { Routes } from '@angular/router';
import { WelcomeComponent } from './welcome';

export const welcomeRoutes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./components/home/home').then((m) => m.HomeComponent),
        title: 'Home',
      },
      {
        path: 'login',
        loadComponent: () => import('./components/login/login').then((m) => m.LoginComponent),
        title: 'Login',
      },
    ],
  },
];
