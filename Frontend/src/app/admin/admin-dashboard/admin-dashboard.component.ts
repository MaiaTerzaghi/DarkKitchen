import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../backend/services/order/order.service';
import TopProductResponse from '../../../backend/services/order/models/TopProductResponse';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  standalone: false,
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit {
  topProducts: TopProductResponse[] = [];
  loadingTop: boolean = false;
  errorTop: string = '';

  dateFrom: string = '';
  dateTo: string = '';

  constructor(private readonly _orderService: OrderService) {}

  ngOnInit(): void {
    this.setDefaultDates();
    this.loadTopProducts();
  }

  private setDefaultDates(): void {
    const today = new Date();
    const monthAgo = new Date();
    monthAgo.setMonth(monthAgo.getMonth() - 1);

    this.dateTo = this.formatDateInput(today);
    this.dateFrom = this.formatDateInput(monthAgo);
  }

  private formatDateInput(date: Date): string {
    const yyyy = date.getFullYear();
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  public loadTopProducts(): void {
    if (!this.dateFrom || !this.dateTo) {
      this.errorTop = 'Las fechas son obligatorias';
      return;
    }

    this.loadingTop = true;
    this.errorTop = '';

    this._orderService.getTopProducts(this.dateFrom, this.dateTo).subscribe({
      next: (data) => {
        this.topProducts = data;
        this.loadingTop = false;
      },
      error: (err) => {
        this.errorTop = err || 'Error al cargar productos más vendidos';
        this.loadingTop = false;
      },
    });
  }

  public getImageList(images: string): string[] {
    if (!images) return [];
    return images.split(',').map((img) => img.trim()).filter((img) => img.length > 0);
  }
}
