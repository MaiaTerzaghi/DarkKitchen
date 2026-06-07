export interface OrderPreviewItemResponse {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  discountPercentage: number;
  discountedUnitPrice: number;
  itemTotal: number;
}

export default interface OrderPreviewResponse {
  items: OrderPreviewItemResponse[];
  subtotal: number;
  discount: number;
  vat: number;
  shippingCost: number;
  total: number;
}
