import { Routes, RouterModule } from '@angular/router';
import { ProfileComponent } from './pages/profile/profile.component';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: 'profile',
    component: ProfileComponent
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
export class UserAccountRoutingModule {}
