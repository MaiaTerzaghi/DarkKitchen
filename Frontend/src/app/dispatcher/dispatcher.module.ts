import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DispatcherRoutingModule } from './dispatcher-routing.module';
import { DispatcherLayoutComponent } from './dispatcher-layout/dispatcher-layout.component';
import { DispatcherDashboardComponent } from './dispatcher-dashboard/dispatcher-dashboard.component';
import { BusinessComponentsModule } from '../business-components/business-components.module';

@NgModule({
  declarations: [DispatcherLayoutComponent, DispatcherDashboardComponent],
  imports: [CommonModule, FormsModule, DispatcherRoutingModule, BusinessComponentsModule],
})
export class DispatcherModule {}
