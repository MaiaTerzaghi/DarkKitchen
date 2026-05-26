import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { CheckoutComponent } from './checkout/checkout.component';

const routes: Routes = [
  {
    path: '',
    component: CustomerLayoutComponent,
    children: [
      {
        path: '',
        component: CustomerDashboardComponent,
      },
      {
        path: 'catalog',
        component: ProductCatalogComponent,
      },
      {
        path: 'checkout',
        component: CheckoutComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CustomerRoutingModule {}
