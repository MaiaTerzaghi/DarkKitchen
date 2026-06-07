import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminOrderListComponent } from './admin-order-list/admin-order-list.component';
import { PromotionListComponent } from '../business-components/promotion-list/promotion-list.component';
import { ProductListComponent } from '../business-components/product-list/product-list.component';
import { AuditLogListComponent } from '../business-components/audit-log-list/audit-log-list.component';
import { UserListComponent } from '../business-components/user-list/user-list.component';
import { AdminSalesReportComponent } from './admin-sales-report/admin-sales-report.component';
import { AdminShippingTypesComponent } from './admin-shipping-types/admin-shipping-types.component';
import { CanDeactivateGuard } from '../../guards/can-deactivate.guard';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        component: AdminDashboardComponent,
      },
      {
        path: 'orders',
        component: AdminOrderListComponent,
      },
      {
        path: 'promotions',
        component: PromotionListComponent,
        canDeactivate: [CanDeactivateGuard],
      },
      {
        path: 'products',
        component: ProductListComponent,
        canDeactivate: [CanDeactivateGuard],
      },
      {
        path: 'audit',
        component: AuditLogListComponent,
      },
      {
        path: 'users',
        component: UserListComponent,
      },
      {
        path: 'report',
        component: AdminSalesReportComponent,
      },
      {
        path: 'shipping-types',
        component: AdminShippingTypesComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}