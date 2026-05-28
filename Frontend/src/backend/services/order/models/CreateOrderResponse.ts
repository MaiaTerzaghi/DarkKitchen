export default interface CreateOrderResponse {
  clientId: number;
  orderId: number;
  subtotal: number;
  vat: number;
  shippingCost: number;
  total: number;
}
