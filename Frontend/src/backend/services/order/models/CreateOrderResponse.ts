export default interface CreateOrderResponse {
  clientId: number;
  orderId: number;
  subtotal: number;
  shippingCost: number;
  total: number;
}
