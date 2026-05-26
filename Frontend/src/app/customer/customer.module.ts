import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { CheckoutComponent } from './checkout/checkout.component';

@NgModule({
  declarations: [CustomerLayoutComponent, CustomerDashboardComponent, ProductCatalogComponent, CheckoutComponent],
  imports: [CommonModule, CustomerRoutingModule, FormsModule, ReactiveFormsModule],
})
export class CustomerModule {}
