import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PromotionListComponent } from './promotion-list/promotion-list.component';
import { PromotionFormComponent } from './promotion-form/promotion-form.component';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductFormComponent } from './product-form/product-form.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { OrderListComponent } from './order-list/order-list.component';
import { AuditLogListComponent } from './audit-log-list/audit-log-list.component';
import { PromotionProductsComponent } from './promotion-products/promotion-products.component';
import { OrderByDateComponent } from './order-by-date/order-by-date.component';
import { OrderManagementComponent } from './order-management/order-management.component';
import { UserListComponent } from './user-list/user-list.component';
import { ConfirmExitModalComponent } from './confirm-exit-modal/confirm-exit-modal.component';
import { PaginatorComponent } from '../components/paginator/paginator.component';

@NgModule({
  declarations: [
    PromotionListComponent,
    PromotionFormComponent,
    PromotionProductsComponent,
    ProductListComponent,
    ProductFormComponent,
    ProductCatalogComponent,
    OrderListComponent,
    AuditLogListComponent,
    OrderByDateComponent,
    OrderManagementComponent,
    UserListComponent,
    ConfirmExitModalComponent,
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginatorComponent],
  exports: [
    PromotionListComponent,
    PromotionFormComponent,
    PromotionProductsComponent,
    ProductListComponent,
    ProductFormComponent,
    ProductCatalogComponent,
    OrderListComponent,
    AuditLogListComponent,
    OrderByDateComponent,
    OrderManagementComponent,
    UserListComponent,
    ConfirmExitModalComponent,
  ],
})
export class BusinessComponentsModule {}