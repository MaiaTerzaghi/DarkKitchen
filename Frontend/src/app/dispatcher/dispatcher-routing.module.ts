import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DispatcherLayoutComponent } from './dispatcher-layout/dispatcher-layout.component';
import { DispatcherDashboardComponent } from './dispatcher-dashboard/dispatcher-dashboard.component';

const routes: Routes = [
  {
    path: '',
    component: DispatcherLayoutComponent,
    children: [
      {
        path: '',
        component: DispatcherDashboardComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DispatcherRoutingModule {}
