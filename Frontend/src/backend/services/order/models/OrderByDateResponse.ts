export interface OrderClientInfo {
  id: number;
  name: string;
  lastName: string;
  phone: string;
}

export interface OrderItemInfo {
  productName: string;
  quantity: number;
}

export default interface OrderByDateResponse {
  orderId: number;
  client: OrderClientInfo;
  date: string;
  status: string;
  items: OrderItemInfo[];
}
