export interface Order {
  id: number;
  totalAmount: number;
  status: string;
  createdAt: string;
  orderItems: OrderItem[];
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}