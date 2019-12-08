import { Routes } from '@angular/router';
import { AddRecipeComponent } from './add-recipe/add-recipe.component';

export const RecipeManagementRoutes: Routes = [
  {
    path: '',
    component: AddRecipeComponent
  }
];
