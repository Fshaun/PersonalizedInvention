<<<<<<< HEAD
=======
export interface DeliveryAddress {
  fullName: string;
  phone: string;
  street: string;
  city: string;
  province: string;
  postalCode: string;
  country: string;
}

>>>>>>> beta
export interface Order {
  id: number;
  totalAmount: number;
  status: string;
  createdAt: string;
<<<<<<< HEAD
=======
  deliveryAddress: DeliveryAddress;
>>>>>>> beta
  orderItems: OrderItem[];
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}