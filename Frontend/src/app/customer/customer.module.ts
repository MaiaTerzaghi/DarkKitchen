import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';

@NgModule({
  declarations: [CustomerLayoutComponent, CustomerDashboardComponent],
  imports: [CommonModule, CustomerRoutingModule],
})
export class CustomerModule {}
