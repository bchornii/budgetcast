import { Component, OnInit } from '@angular/core';
import { RecipeService } from '../services/receipt.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-receipt',
  templateUrl: './add-receipt.component.html'
})
export class AddReceiptComponent implements OnInit {
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
