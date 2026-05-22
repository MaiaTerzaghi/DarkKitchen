import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DispatcherRoutingModule } from './dispatcher-routing.module';
import { DispatcherLayoutComponent } from './dispatcher-layout/dispatcher-layout.component';
import { DispatcherDashboardComponent } from './dispatcher-dashboard/dispatcher-dashboard.component';

@NgModule({
  declarations: [DispatcherLayoutComponent, DispatcherDashboardComponent],
  imports: [CommonModule, DispatcherRoutingModule],
})
export class DispatcherModule {}
