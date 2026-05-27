export default interface OrderResponse {
  orderId: number;
  clientId: number;
  date: string;
  status: string;
  total: number;
  itemCount: number;
}