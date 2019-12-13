import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AccountModule } from './account/account.module';
import { RecipeManagementModule } from './receipt-management/receipt-management.module';

export const appRoutes: Routes = [
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'account',
    loadChildren: () => import('./account/account.module').then(m => AccountModule)
  },
  {
    path: 'receipt',
    loadChildren: () => import('./receipt-management/receipt-management.module').then(m => RecipeManagementModule)
  },
  {
    path: '',
    redirectTo: 'receipt',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/home',
    pathMatch: 'full'
  }
];
