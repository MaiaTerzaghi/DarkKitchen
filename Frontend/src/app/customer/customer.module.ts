import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerRoutingModule } from './customer-routing.module';
import { BusinessComponentsModule } from '../business-components/business-components.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';

@NgModule({
  declarations: [CustomerLayoutComponent, CustomerDashboardComponent],
  imports: [CommonModule, CustomerRoutingModule, BusinessComponentsModule],
})
export class CustomerModule {}
