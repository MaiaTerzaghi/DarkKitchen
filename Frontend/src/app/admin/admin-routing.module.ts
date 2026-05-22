import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { PromotionListComponent } from '../business-components/promotion-list/promotion-list.component';

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
        path: 'promotions',
        component: PromotionListComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
