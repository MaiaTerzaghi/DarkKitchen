import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../backend/services/product/product.service';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';
import ProductManageFilter from '../../../backend/services/product/models/ProductManageFilter';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  standalone: false,
  styleUrls: ['./product-list.component.css'],
})
export class ProductListComponent implements OnInit {
  products: ProductResponse[] = [];
  filteredProducts: ProductResponse[] = [];
  errorMessage: string = '';
  loading: boolean = false;
  showFilters: boolean = false;
  showForm: boolean = false;
  selectedProduct: ProductResponse | null = null;
  searchText: string = '';
  isAdmin: boolean = false;

  filterCategory: string = '';
  filterCommercialLine: string = '';
  filterActive: string = '';
  filterPriceMin: number | null = null;
  filterPriceMax: number | null = null;

  constructor(
    private readonly _productService: ProductService,
    private readonly _authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this._authService.getRole() === 'Administrative';
    this.loadProducts();
  }

  public loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    const filters: ProductManageFilter = {};
    if (this.filterCategory) {
      filters.category = this.filterCategory;
    }
    if (this.filterCommercialLine) {
      filters.commercialLine = this.filterCommercialLine;
    }
    if (this.filterActive) {
      filters.isActive = this.filterActive === 'true';
    }
    if (this.filterPriceMin != null) {
      filters.priceMin = this.filterPriceMin;
    }
    if (this.filterPriceMax != null) {
      filters.priceMax = this.filterPriceMax;
    }

    this._productService.getManage(filters).subscribe({
      next: (data) => {
        this.products = data;
        this.applySearch();
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar productos';
        this.loading = false;
      },
    });
  }

  public applySearch(): void {
    if (!this.searchText) {
      this.filteredProducts = this.products;
      return;
    }

    const search = this.searchText.toLowerCase();
    this.filteredProducts = this.products.filter(
      (p) =>
        p.name.toLowerCase().includes(search) ||
        p.code.toLowerCase().includes(search) ||
        p.category.toLowerCase().includes(search)
    );
  }

  public toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  public clearFilters(): void {
    this.filterCategory = '';
    this.filterCommercialLine = '';
    this.filterActive = '';
    this.filterPriceMin = null;
    this.filterPriceMax = null;
    this.searchText = '';
    this.showFilters = false;
    this.loadProducts();
  }

  public openCreateForm(): void {
    this.selectedProduct = null;
    this.showForm = true;
  }

  public openEditForm(product: ProductResponse): void {
    this.selectedProduct = product;
    this.showForm = true;
  }

  public onFormClose(): void {
    this.showForm = false;
    this.selectedProduct = null;
  }

  public onProductSaved(): void {
    this.showForm = false;
    this.selectedProduct = null;
    this.loadProducts();
  }

  public toggleActive(product: ProductResponse): void {
    this.errorMessage = '';
    this._productService
      .update(product.id, {
        code: product.code,
        name: product.name,
        description: product.description,
        price: product.price,
        commercialLine: product.commercialLine,
        category: product.category,
        images: product.images,
        isActive: !product.isActive,
      })
      .subscribe({
        next: () => this.loadProducts(),
        error: (err) => {
          this.errorMessage = err || 'Error al cambiar el estado del producto';
        },
      });
  }
}