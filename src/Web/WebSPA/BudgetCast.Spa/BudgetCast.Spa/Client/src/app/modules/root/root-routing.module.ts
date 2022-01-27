import { Routes, RouterModule } from '@angular/router';
import { RootComponent } from './root.component';
import { NgModule } from '@angular/core';

export const routes: Routes = [
  {
    path: '',
    component: RootComponent,
    children: [
      {
        path: 'expenses',
        loadChildren: () => import('../expense-management/expense-management.module').then(m => m.ExpenseManagementModule)
      },
      {
        path: 'user-account',
        loadChildren: () => import('../user-account/user-account.module').then(m => m.UserAccountModule)
      },
      {
        path: '',
        redirectTo: 'expenses',
        pathMatch: 'full'
      }
    ]
  }
];
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class RootRoutingModule {}
