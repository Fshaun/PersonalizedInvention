import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AdminProduct {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  category: string;
  isInStock: boolean;
  createdAt: string;
}

export interface AdminOrder {
  id: number;
  userId: number;
  userEmail: string;
  userFullName: string;
  totalAmount: number;
  status: string;
  createdAt: string;
  orderItems: AdminOrderItem[];
}

export interface AdminOrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface ProductFormData {
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  category: string;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  // ── Products ──────────────────────────────────────────────────
  getAllProducts(): Observable<AdminProduct[]> {
    return this.http.get<AdminProduct[]>(`${this.apiUrl}/products`);
  }

  createProduct(data: ProductFormData): Observable<AdminProduct> {
    return this.http.post<AdminProduct>(`${this.apiUrl}/products`, data);
  }

  updateProduct(id: number, data: ProductFormData): Observable<AdminProduct> {
    return this.http.put<AdminProduct>(`${this.apiUrl}/products/${id}`, data);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${id}`);
  }

  updateStock(id: number, stock: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/products/${id}/stock`, { stock });
  }

  // ── Orders ────────────────────────────────────────────────────
  getAllOrders(): Observable<AdminOrder[]> {
    return this.http.get<AdminOrder[]>(`${this.apiUrl}/orders`);
  }

  updateOrderStatus(id: number, status: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/orders/${id}/status`, { status });
  }
}