<<<<<<< HEAD
=======
export const PRODUCT_CATEGORIES = [
  'Notebook',
  'Keyholder',
  'Frame',
  'Letter',
  'Board',
  'Family Tree',
  'Badge',
  'Bottle'
];

>>>>>>> beta
export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  category: string;
  isInStock: boolean;
}