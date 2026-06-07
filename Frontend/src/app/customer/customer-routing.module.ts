import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { PromotionListComponent } from '../business-components/promotion-list/promotion-list.component';
import { ProductCatalogComponent } from './product-catalog/product-catalog.component';
import { OrderListComponent } from '../business-components/order-list/order-list.component';
import { CheckoutComponent } from './checkout/checkout.component';

const routes: Routes = [
  {
    path: '',
    component: CustomerLayoutComponent,
    children: [
      {
        path: '',
        component: ProductCatalogComponent,
      },
      {
        path: 'promotions',
        component: PromotionListComponent,
      },
      {
        path: 'orders',
        component: OrderListComponent,
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