import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../backend/services/product/product.service';
import { CartService } from '../services/cart.service';
import { PromotionService } from '../../../backend/services/promotion/promotion.service';
import PromotionResponse from '../../../backend/services/promotion/models/PromotionResponse';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

@Component({
  selector: 'app-product-catalog',
  templateUrl: './product-catalog.component.html',
  standalone: false,
  styleUrls: ['./product-catalog.component.css'],
})
export class ProductCatalogComponent implements OnInit {
  products: ProductResponse[] = [];
  promotions: PromotionResponse[] = [];
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
    private readonly _cartService: CartService,
    private readonly _promotionService: PromotionService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadPromotions();
  }

  private loadProducts(): void {
    this.loading = true;
    this._productService.getProducts().subscribe({
      next: (data) => {
        this.products = data.items;
        this.totalCount = data.totalCount;
        this.categories = [
          'Todos',
          ...new Set(data.items.map((p) => p.category)),
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

  private loadPromotions(): void {
    this._promotionService.getActivePromotions({ date: new Date().toISOString().split('T')[0] }).subscribe({
      next: (data) => {
        this.promotions = data.items;
      },
    });
  }

  getActivePromotion(product: ProductResponse): PromotionResponse | null {
    return this.promotions.find(p =>
      p.products?.some(prod => prod.id === product.id)
    ) || null;
  }

  getDiscountedPrice(product: ProductResponse): number | null {
    const promo = this.getActivePromotion(product);
    if (!promo) return null;
    return product.price * (1 - Number(promo.discountPercentage) / 100);
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
      result = result.filter((p) =>
        p.name.toLowerCase().includes(term) ||
        p.category.toLowerCase().includes(term) ||
        p.commercialLine.toLowerCase().includes(term)
      );
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
