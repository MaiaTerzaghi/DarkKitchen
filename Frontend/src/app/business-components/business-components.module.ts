import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromotionListComponent } from './promotion-list/promotion-list.component';

@NgModule({
  declarations: [PromotionListComponent],
  imports: [CommonModule, FormsModule],
  exports: [PromotionListComponent],
})
export class BusinessComponentsModule {}
