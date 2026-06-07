import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminRoutingModule } from './admin-routing.module';
import { BusinessComponentsModule } from '../business-components/business-components.module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminOrderListComponent } from './admin-order-list/admin-order-list.component';
import { AdminSalesReportComponent } from './admin-sales-report/admin-sales-report.component';
import { AdminShippingTypesComponent } from './admin-shipping-types/admin-shipping-types.component';

@NgModule({
  declarations: [AdminLayoutComponent, AdminDashboardComponent, AdminOrderListComponent, AdminSalesReportComponent, AdminShippingTypesComponent],
  imports: [CommonModule, AdminRoutingModule, FormsModule, BusinessComponentsModule],
})
export class AdminModule {}
