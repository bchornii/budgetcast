import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddRecipeComponent } from './add-recipe/add-recipe.component';
import { RouterModule } from '@angular/router';
import { RecipeManagementRoutes } from './recipe-management.routes';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AddRecipeComponent
  ],
  imports: [
    CommonModule,
    FormsModule,

    RouterModule.forChild(RecipeManagementRoutes),

    SharedModule,
  ]
})
export class RecipeManagementModule { }
