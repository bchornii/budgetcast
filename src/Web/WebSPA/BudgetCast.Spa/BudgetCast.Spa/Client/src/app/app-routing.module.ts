import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { UserAuthenticatedGuard } from './guards/user-authenticated.guard';

export const appRoutes: Routes = [
  {
    path: 'welcome',
    loadChildren: () => import('./modules/welcome/welcome.module').then(m => m.WelcomeModule)
  },
  {
    path: '',
    loadChildren: () => import('./modules/root/root.module').then(m => m.RootModule),
    canActivate: [UserAuthenticatedGuard]
  },
  {
    path: '**',
    redirectTo: '/welcome',
    pathMatch: 'full'
  }
];
@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
