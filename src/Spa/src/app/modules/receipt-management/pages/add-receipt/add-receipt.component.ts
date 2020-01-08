import { Component, OnInit } from '@angular/core';
import { RecipeService } from '../../services/receipt.service';
import { finalize } from 'rxjs/operators';

import * as moment from 'moment';
import { Moment } from 'moment';


@Component({
  selector: 'app-add-receipt',
  templateUrl: './add-receipt.component.html'
})
export class AddReceiptComponent implements OnInit {
  categories: string[];
  initialCategoryName = 'Food';
  categoriesIsLoading = false;

  tags = ['Food', 'Healthy Food'];
  receiptDate: Moment = moment(); 

  constructor(private recipeService: RecipeService) { }

  ngOnInit() { 
    //this.inputControl.valueChanges
    //  .subscribe((val: Moment) => console.log(val.format("MM-DD-YYYY")));
  }

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
