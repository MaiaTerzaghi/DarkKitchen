import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../backend/services/product/product.service';
import { CartService } from '../services/cart.service';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

@Component({
  selector: 'app-product-catalog',
  templateUrl: './product-catalog.component.html',
  standalone: false,
  styleUrls: ['./product-catalog.component.css'],
})
export class ProductCatalogComponent implements OnInit {
  products: ProductResponse[] = [];
  filteredProducts: ProductResponse[] = [];
  categories: string[] = [];
  selectedCategory: string = 'Todos';
  searchTerm: string = '';
  loading: boolean = true;
  errorMessage: string = '';

  // Paginación
  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;

  // Carrusel: índice de imagen actual por producto
  private imageIndexMap: { [productId: number]: number } = {};

  constructor(
    private readonly _productService: ProductService,
    private readonly _cartService: CartService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.loading = true;
    this._productService.getAll(undefined, undefined, undefined, this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.products = response.items;
        this.totalCount = response.totalCount;
        this.categories = [
          'Todos',
          ...new Set(response.items.map((p) => p.category)),
        ];
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Error al cargar productos';
        this.loading = false;
      },
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadProducts();
  }

  filterByCategory(category: string): void {
    this.selectedCategory = category;
    this.applyFilters();
  }

  onSearch(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    let result = this.products;
    if (this.selectedCategory !== 'Todos') {
      result = result.filter((p) => p.category === this.selectedCategory);
    }
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter((p) => p.name.toLowerCase().includes(term));
    }
    this.filteredProducts = result;
  }

  imageList(images: string): string[] {
    return images
      ? images.split(',').map((i) => i.trim()).filter((i) => i.length > 0)
      : [];
  }

  getImageIndex(productId: number): number {
    return this.imageIndexMap[productId] || 0;
  }

  setImageIndex(productId: number, index: number): void {
    this.imageIndexMap[productId] = index;
  }

  nextImage(product: ProductResponse): void {
    const images = this.imageList(product.images);
    const current = this.getImageIndex(product.id);
    this.imageIndexMap[product.id] = (current + 1) % images.length;
  }

  prevImage(product: ProductResponse): void {
    const images = this.imageList(product.images);
    const current = this.getImageIndex(product.id);
    this.imageIndexMap[product.id] = (current - 1 + images.length) % images.length;
  }

  addToCart(product: ProductResponse): void {
    this._cartService.addItem(product);
  }

  getCartCount(): number {
    return this._cartService.getItemCount();
  }
}
