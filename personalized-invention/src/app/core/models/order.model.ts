export interface DeliveryAddress {
  fullName: string;
  phone: string;
  street: string;
  city: string;
  province: string;
  postalCode: string;
  country: string;
}

export interface Order {
  id: number;
  totalAmount: number;
  status: string;
  createdAt: string;
  deliveryAddress: DeliveryAddress;
  orderItems: OrderItem[];
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}