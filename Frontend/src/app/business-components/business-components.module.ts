import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PromotionListComponent } from './promotion-list/promotion-list.component';
import { PromotionFormComponent } from './promotion-form/promotion-form.component';

@NgModule({
  declarations: [PromotionListComponent, PromotionFormComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  exports: [PromotionListComponent, PromotionFormComponent],
})
export class BusinessComponentsModule {}
