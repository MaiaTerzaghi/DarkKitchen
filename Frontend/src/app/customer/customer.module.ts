import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CustomerRoutingModule } from './customer-routing.module';
import { BusinessComponentsModule } from '../business-components/business-components.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { CheckoutComponent } from './checkout/checkout.component';

@NgModule({
  declarations: [CustomerLayoutComponent, ProductCatalogComponent, CheckoutComponent],
  imports: [CommonModule, CustomerRoutingModule, FormsModule, ReactiveFormsModule, BusinessComponentsModule],
})
export class CustomerModule {}
