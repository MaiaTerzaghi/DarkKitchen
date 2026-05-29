import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CartService, CartItem } from '../services/cart.service';
import { OrderService } from '../../../backend/services/order/order.service';
import { ShippingTypeService } from '../../../backend/services/shipping-type/shipping-type.service';
import ShippingTypeResponse from '../../../backend/services/shipping-type/models/ShippingTypeResponse';
import CreateOrderResponse from '../../../backend/services/order/models/CreateOrderResponse';
import OrderPreviewResponse, { OrderPreviewItemResponse } from '../../../backend/services/order/models/OrderPreviewResponse';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  standalone: false,
  styleUrls: ['./checkout.component.css'],
})
export class CheckoutComponent implements OnInit {
  addressForm!: FormGroup;
  shippingTypes: ShippingTypeResponse[] = [];
  selectedShippingType: ShippingTypeResponse | null = null;

  preview: OrderPreviewResponse | null = null;
  loadingPreview: boolean = false;

  loading: boolean = false;
  loadingShipping: boolean = true;
  errorMessage: string = '';
  orderSuccess: boolean = false;
  orderResponse: CreateOrderResponse | null = null;

  constructor(
    private readonly _fb: FormBuilder,
    private readonly _cartService: CartService,
    private readonly _orderService: OrderService,
    private readonly _shippingTypeService: ShippingTypeService
  ) {}

  ngOnInit(): void {
    this.addressForm = this._fb.group({
      street: ['', Validators.required],
      doorNumber: ['', Validators.required],
      apartment: [''],
    });

    this.loadShippingTypes();
  }

  private loadShippingTypes(): void {
    this.loadingShipping = true;
    this._shippingTypeService.getAll().subscribe({
      next: (types) => {
        this.shippingTypes = types;
        if (types.length > 0) {
          this.selectedShippingType = types[0];
          this.loadPreview();
        }
        this.loadingShipping = false;
      },
      error: () => {
        this.errorMessage = 'Error al cargar tipos de envío';
        this.loadingShipping = false;
      },
    });
  }

  loadPreview(): void {
    if (this.isCartEmpty || !this.selectedShippingType) return;

    this.loadingPreview = true;
    const items = this.cartItems.map((item) => ({
      productId: item.product.id,
      quantity: item.quantity,
    }));

    this._orderService.previewOrder(items, this.selectedShippingType.name).subscribe({
      next: (result) => {
        this.preview = result;
        this.loadingPreview = false;
      },
      error: () => {
        this.errorMessage = 'Error al calcular el resumen del pedido';
        this.loadingPreview = false;
      },
    });
  }

  get cartItems(): CartItem[] {
    return this._cartService.getItems();
  }

  get itemCount(): number {
    return this._cartService.getItemCount();
  }

  get isCartEmpty(): boolean {
    return this.cartItems.length === 0;
  }

  get canSubmit(): boolean {
    return (
      !this.isCartEmpty &&
      this.addressForm.valid &&
      this.selectedShippingType !== null &&
      this.preview !== null &&
      !this.loading
    );
  }

  getPreviewItem(productId: number): OrderPreviewItemResponse | undefined {
    return this.preview?.items.find((i) => i.productId === productId);
  }

  selectShippingType(type: ShippingTypeResponse): void {
    this.selectedShippingType = type;
    this.loadPreview();
  }

  incrementQuantity(item: CartItem): void {
    this._cartService.updateQuantity(item.product.code, item.quantity + 1);
    this.loadPreview();
  }

  decrementQuantity(item: CartItem): void {
    this._cartService.updateQuantity(item.product.code, item.quantity - 1);
    this.loadPreview();
  }

  removeItem(item: CartItem): void {
    this._cartService.removeItem(item.product.code);
    this.loadPreview();
  }

  onSubmit(): void {
    if (!this.canSubmit) return;

    this.loading = true;
    this.errorMessage = '';

    const formValues = this.addressForm.value;

    const request = {
      shippingType: this.selectedShippingType!.name,
      address: {
        street: formValues.street,
        doorNumber: formValues.doorNumber,
        apartment: formValues.apartment || undefined,
      },
      items: this.cartItems.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity,
      })),
    };

    this._orderService.createOrder(request).subscribe({
      next: (response) => {
        this.orderResponse = response;
        this.orderSuccess = true;
        this._cartService.clear();
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al crear el pedido. Intenta nuevamente.';
        this.loading = false;
      },
    });
  }
}
