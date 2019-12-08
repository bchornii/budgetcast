import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AccountModule } from './account/account.module';
import { RecipeManagementModule } from './recipe-management/recipe-management.module';

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
    path: 'recipe',
    loadChildren: () => import('./recipe-management/recipe-management.module').then(m => RecipeManagementModule)
  },
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/home',
    pathMatch: 'full'
  }
];
