import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { PromotionService } from '../../../backend/services/promotion/promotion.service';
import { ProductService } from '../../../backend/services/product/product.service';
import PromotionResponse from '../../../backend/services/promotion/models/PromotionResponse';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

@Component({
  selector: 'app-promotion-products',
  templateUrl: './promotion-products.component.html',
  standalone: false,
  styleUrls: ['./promotion-products.component.css'],
})
export class PromotionProductsComponent implements OnChanges {
  @Input() visible: boolean = false;
  @Input() promotion: PromotionResponse | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() changed = new EventEmitter<void>();

  availableProducts: ProductResponse[] = [];
  searchText: string = '';
  loading: boolean = false;
  errorMessage: string = '';

  constructor(
    private readonly _promotionService: PromotionService,
    private readonly _productService: ProductService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible && this.promotion) {
      this.searchText = '';
      this.availableProducts = [];
      this.errorMessage = '';
    }
  }

  public searchProducts(): void {
    if (!this.searchText.trim()) {
      return;
    }

    this.loading = true;
    this._productService.getProducts({ name: this.searchText }).subscribe({
      next: (products) => {
        // Filtrar los que ya están asociados
        const associatedIds = this.promotion?.products?.map((p) => p.id) || [];
        this.availableProducts = products.items.filter(
          p => !associatedIds.includes(p.id)
        );
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al buscar productos';
        this.loading = false;
      },
    });
  }

  public addProduct(product: ProductResponse): void {
    if (!this.promotion) return;

    this._promotionService
      .addProduct(this.promotion.id, product.id)
      .subscribe({
        next: () => {
          // Agregar al array local para actualizar la vista
          this.promotion!.products.push({ id: product.id, name: product.name });
          // Quitar de disponibles
          this.availableProducts = this.availableProducts.filter(
            (p) => p.id !== product.id
          );
          this.changed.emit();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al asociar producto';
        },
      });
  }

  public removeProduct(productId: number): void {
    if (!this.promotion) return;

    this._promotionService
      .removeProduct(this.promotion.id, productId)
      .subscribe({
        next: () => {
          // Quitar del array local
          this.promotion!.products = this.promotion!.products.filter(
            (p) => p.id !== productId
          );
          this.changed.emit();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al desasociar producto';
        },
      });
  }

  public onClose(): void {
    this.close.emit();
  }
}
