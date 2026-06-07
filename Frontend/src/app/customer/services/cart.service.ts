import { Injectable } from '@angular/core';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

export interface CartItem {
  product: ProductResponse;
  quantity: number;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private items: CartItem[] = [];

  getItems(): CartItem[] {
    return this.items;
  }

  getItemCount(): number {
    return this.items.reduce((sum, item) => sum + item.quantity, 0);
  }

  getSubtotal(): number {
    return this.items.reduce(
      (sum, item) => sum + item.product.price * item.quantity,
      0
    );
  }

  addItem(product: ProductResponse): void {
    const existing = this.items.find((i) => i.product.code === product.code);
    if (existing) {
      existing.quantity++;
    } else {
      this.items.push({ product, quantity: 1 });
    }
  }

  removeItem(productCode: string): void {
    this.items = this.items.filter((i) => i.product.code !== productCode);
  }

  updateQuantity(productCode: string, quantity: number): void {
    const item = this.items.find((i) => i.product.code === productCode);
    if (item) {
      if (quantity <= 0) {
        this.removeItem(productCode);
      } else {
        item.quantity = quantity;
      }
    }
  }

  clear(): void {
    this.items = [];
  }
}
