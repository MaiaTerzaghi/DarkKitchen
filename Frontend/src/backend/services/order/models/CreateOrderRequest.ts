export default interface CreateOrderRequest {
  shippingType: string;
  address: {
    street: string;
    doorNumber: string;
    apartment?: string;
  };
  items: {
    productId: number;
    quantity: number;
  }[];
}
