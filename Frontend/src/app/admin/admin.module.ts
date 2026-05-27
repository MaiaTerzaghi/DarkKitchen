import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminRoutingModule } from './admin-routing.module';
import { BusinessComponentsModule } from '../business-components/business-components.module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';

@NgModule({
  declarations: [AdminLayoutComponent, AdminDashboardComponent],
  imports: [CommonModule, AdminRoutingModule, BusinessComponentsModule],
})
export class AdminModule {}
