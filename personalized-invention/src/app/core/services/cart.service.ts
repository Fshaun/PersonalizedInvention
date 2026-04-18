import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { CartItem, CartResponse } from '../models/cart-item.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = `${environment.apiUrl}/cart`;

  // BehaviorSubject lets any component subscribe to live cart count updates
  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  // Hardcoded userId = 1 for now (replace with auth service later)
  private userId = 1;

  constructor(private http: HttpClient) {
    this.loadCartCount();
  }

  getCart(): Observable<CartResponse> {
    return this.http.get<CartResponse>(`${this.apiUrl}/${this.userId}`).pipe(
      tap(res => this.cartCountSubject.next(res.items.length))
    );
  }

  addItem(productId: number, quantity: number = 1): Observable<CartItem> {
    return this.http.post<CartItem>(`${this.apiUrl}/${this.userId}/add`, { productId, quantity }).pipe(
      tap(() => this.cartCountSubject.next(this.cartCountSubject.value + 1))
    );
  }

  updateItem(productId: number, quantity: number): Observable<CartItem> {
    return this.http.put<CartItem>(`${this.apiUrl}/${this.userId}/update`, { productId, quantity });
  }

  removeItem(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${this.userId}/remove/${productId}`).pipe(
      tap(() => this.cartCountSubject.next(Math.max(0, this.cartCountSubject.value - 1)))
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${this.userId}/clear`).pipe(
      tap(() => this.cartCountSubject.next(0))
    );
  }

  private loadCartCount(): void {
    this.getCart().subscribe({ error: () => {} });
  }
}