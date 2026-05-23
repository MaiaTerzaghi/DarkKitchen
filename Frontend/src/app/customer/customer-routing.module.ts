import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';
import { PromotionListComponent } from '../business-components/promotion-list/promotion-list.component';
import { ProductCatalogComponent } from '../business-components/product-catalog/product-catalog.component';

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
        path: 'promotions',
        component: PromotionListComponent,
      },
      {
        path: 'catalog',
        component: ProductCatalogComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CustomerRoutingModule {}