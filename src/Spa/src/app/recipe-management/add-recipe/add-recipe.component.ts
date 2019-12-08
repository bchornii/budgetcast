import { Component, OnInit } from '@angular/core';
import { RecipeService } from '../recipe.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-recipe',
  templateUrl: './add-recipe.component.html'
})
export class AddRecipeComponent implements OnInit {
  categories: string[];
  initialCategoryName = 'Food';
  categoriesIsLoading = false;

  tags = ['Food', 'Healthy Food'];

  constructor(private recipeService: RecipeService) { }

  ngOnInit() { }

  onCategoryModelChange(data) {
    console.log(this.tags);
    if (!this.categoryExists(data)) {
      this.categoriesIsLoading = true;
      this.recipeService.getCategories(data).pipe(
        finalize(() => this.categoriesIsLoading = false)
      )
      .subscribe(r => this.categories = r);
    }
  }

  private categoryExists(name: string) {
    return this.categories && this.categories.includes(name);
  }

}
