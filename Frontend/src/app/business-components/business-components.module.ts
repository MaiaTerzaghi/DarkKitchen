import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PromotionListComponent } from './promotion-list/promotion-list.component';
import { PromotionFormComponent } from './promotion-form/promotion-form.component';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductFormComponent } from './product-form/product-form.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { OrderListComponent } from './order-list/order-list.component';

@NgModule({
  declarations: [
    PromotionListComponent,
    PromotionFormComponent,
    ProductListComponent,
    ProductFormComponent,
    ProductCatalogComponent,
    OrderListComponent,
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  exports: [
    PromotionListComponent,
    PromotionFormComponent,
    ProductListComponent,
    ProductFormComponent,
    ProductCatalogComponent,
    OrderListComponent,
  ],
})
export class BusinessComponentsModule {}