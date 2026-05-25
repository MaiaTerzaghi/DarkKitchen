import { Component, OnInit } from '@angular/core';
import { PromotionService } from '../../../backend/services/promotion/promotion.service';
import PromotionResponse from '../../../backend/services/promotion/models/PromotionResponse';
import PromotionFilter from '../../../backend/services/promotion/models/PromotionFilter';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-promotion-list',
  templateUrl: './promotion-list.component.html',
  standalone: false,
  styleUrls: ['./promotion-list.component.css'],
})
export class PromotionListComponent implements OnInit {
  promotions: PromotionResponse[] = [];
  filteredPromotions: PromotionResponse[] = [];
  errorMessage: string = '';
  loading: boolean = false;
  showFilters: boolean = false;
  showForm: boolean = false;
  showProducts: boolean = false;
  selectedPromotion: PromotionResponse | null = null;
  productsPromotion: PromotionResponse | null = null;
  searchText: string = '';
  isAdmin: boolean = false;

  // Filtros avanzados
  filterDate: string = '';
  filterProductLine: string = '';
  filterProduct: string = '';

  constructor(
    private readonly _promotionService: PromotionService,
    private readonly _authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this._authService.getRole() === 'Administrative';
    this.loadPromotions();
  }

  public loadPromotions(): void {
    this.loading = true;
    this.errorMessage = '';

    const filters: PromotionFilter = {};

    if (this.filterDate) {
      filters.date = this.filterDate;
    }
    if (this.filterProductLine) {
      filters.productLine = this.filterProductLine;
    }
    if (this.filterProduct) {
      filters.product = this.filterProduct;
    }

    this._promotionService.getActivePromotions(filters).subscribe({
      next: (data) => {
        this.promotions = data;
        this.applySearch();
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar promociones';
        this.loading = false;
      },
    });
  }

  public applySearch(): void {
    if (!this.searchText) {
      this.filteredPromotions = this.promotions;
      return;
    }

    const search = this.searchText.toLowerCase();
    this.filteredPromotions = this.promotions.filter(
      (p) =>
        p.name.toLowerCase().includes(search) ||
        p.products?.some((prod) => prod.name.toLowerCase().includes(search))
    );
  }

  public toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  public clearFilters(): void {
    this.filterDate = '';
    this.filterProductLine = '';
    this.filterProduct = '';
    this.searchText = '';
    this.showFilters = false;
    this.loadPromotions();
  }

  public isVigente(promo: PromotionResponse): boolean {
    const now = new Date();
    return new Date(promo.validFrom) <= now && new Date(promo.validTo) >= now;
  }

  public openCreateForm(): void {
    this.selectedPromotion = null;
    this.showForm = true;
  }

  public openEditForm(promo: PromotionResponse): void {
    this.selectedPromotion = promo;
    this.showForm = true;
  }

  public onFormClose(): void {
    this.showForm = false;
    this.selectedPromotion = null;
  }

  public onPromotionSaved(): void {
    this.showForm = false;
    this.selectedPromotion = null;
    this.loadPromotions();
  }

  public openProducts(promo: PromotionResponse): void {
    this.productsPromotion = promo;
    this.showProducts = true;
  }

  public onProductsClose(): void {
    this.showProducts = false;
    this.productsPromotion = null;
  }

  public onProductsChanged(): void {
    // Recarga para reflejar los cambios en los chips
    this.loadPromotions();
  }
}
