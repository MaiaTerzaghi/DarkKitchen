import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../backend/services/product/product.service';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';
import ProductCatalogFilter from '../../../backend/services/product/models/ProductCatalogFilter';

@Component({
  selector: 'app-product-catalog',
  templateUrl: './product-catalog.component.html',
  standalone: false,
  styleUrls: ['./product-catalog.component.css'],
})
export class ProductCatalogComponent implements OnInit {
  products: ProductResponse[] = [];
  filteredProducts: ProductResponse[] = [];
  errorMessage: string = '';
  loading: boolean = false;
  showFilters: boolean = false;
  searchText: string = '';

  filterCategory: string = '';
  filterLine: string = '';

  constructor(private readonly _productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  public loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    const filters: ProductCatalogFilter = {};
    if (this.filterCategory) {
      filters.category = this.filterCategory;
    }
    if (this.filterLine) {
      filters.line = this.filterLine;
    }

    this._productService.getCatalog(filters).subscribe({
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
        p.category.toLowerCase().includes(search) ||
        p.commercialLine.toLowerCase().includes(search)
    );
  }

  public toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  public clearFilters(): void {
    this.filterCategory = '';
    this.filterLine = '';
    this.searchText = '';
    this.showFilters = false;
    this.loadProducts();
  }

  public imageList(images: string): string[] {
    return images
      ? images.split(',').map((i) => i.trim()).filter((i) => i.length > 0)
      : [];
  }
}