import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DispatcherLayoutComponent } from './dispatcher-layout/dispatcher-layout.component';
import { DispatcherDashboardComponent } from './dispatcher-dashboard/dispatcher-dashboard.component';
import { OrderByDateComponent } from '../business-components/order-by-date/order-by-date.component';
import { OrderManagementComponent } from '../business-components/order-management/order-management.component';

const routes: Routes = [
  {
    path: '',
    component: DispatcherLayoutComponent,
    children: [
      {
        path: '',
        component: DispatcherDashboardComponent,
      },
      {
        path: 'orders',
        component: OrderByDateComponent,
      },
      {
        path: 'management',
        component: OrderManagementComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DispatcherRoutingModule {}