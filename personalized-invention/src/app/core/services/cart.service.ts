import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, of, tap, throwError } from 'rxjs';
import { CartItem, CartResponse } from '../models/cart-item.model';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CartService {
  private apiUrl = `${environment.apiUrl}/cart`;
  private readonly storageKeyPrefix = 'pi_cart_';
  private cartCountSubject = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCountSubject.asObservable();

  constructor(private http: HttpClient, private authService: AuthService) {
    this.authService.currentUser$.subscribe(user => {
      if (!user) {
        this.cartCountSubject.next(0);
        return;
      }

      this.getCart().subscribe();
    });
  }

  // ← Now uses the real logged-in user's ID from AuthService
  private get userId(): number {
    return this.authService.userId;
  }

  getCart(): Observable<CartResponse> {
    return this.http.get<CartResponse>(`${this.apiUrl}/${this.userId}`).pipe(
      tap(res => {
        this.saveCart(res);
        this.cartCountSubject.next(res.items.length);
      }),
      catchError(error => {
        const cachedCart = this.loadCart();
        if (cachedCart) {
          this.cartCountSubject.next(cachedCart.items.length);
          return of(cachedCart);
        }

        return throwError(() => error);
      })
    );
  }

  addItem(productId: number, quantity: number = 1): Observable<CartItem> {
    return this.http.post<CartItem>(
      `${this.apiUrl}/${this.userId}/add`,
      { productId, quantity }
    ).pipe(
      tap(item => {
        const cart = this.loadCart() ?? { items: [], total: 0 };
        const existingItem = cart.items.find(existing => existing.productId === item.productId);

        if (existingItem) {
          existingItem.quantity = item.quantity;
          existingItem.subtotal = item.subtotal;
        } else {
          cart.items.push(item);
        }

        cart.total = cart.items.reduce((total, cartItem) => total + cartItem.subtotal, 0);
        this.saveCart(cart);
        this.cartCountSubject.next(cart.items.length);
      })
    );
  }

  updateItem(productId: number, quantity: number): Observable<CartItem> {
    return this.http.put<CartItem>(
      `${this.apiUrl}/${this.userId}/update`,
      { productId, quantity }
    ).pipe(
      tap(item => {
        const cart = this.loadCart();
        const existingItem = cart?.items.find(existing => existing.productId === item.productId);
        if (!cart || !existingItem) return;

        if (item.quantity <= 0) {
          cart.items = cart.items.filter(cartItem => cartItem.productId !== item.productId);
        } else {
          existingItem.quantity = item.quantity;
          existingItem.subtotal = item.subtotal;
        }
        cart.total = cart.items.reduce((total, cartItem) => total + cartItem.subtotal, 0);
        this.saveCart(cart);
      })
    );
  }

  removeItem(productId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${this.userId}/remove/${productId}`
    ).pipe(
      tap(() => {
        const cart = this.loadCart();
        if (!cart) return;

        cart.items = cart.items.filter(item => item.productId !== productId);
        cart.total = cart.items.reduce((total, cartItem) => total + cartItem.subtotal, 0);
        this.saveCart(cart);
        this.cartCountSubject.next(cart.items.length);
      })
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${this.userId}/clear`).pipe(
      tap(() => {
        this.removeSavedCart();
        this.cartCountSubject.next(0);
      })
    );
  }

  private saveCart(cart: CartResponse): void {
    if (this.userId === 0) return;
    localStorage.setItem(`${this.storageKeyPrefix}${this.userId}`, JSON.stringify(cart));
  }

  private loadCart(): CartResponse | null {
    if (this.userId === 0) return null;

    try {
      const storedCart = localStorage.getItem(`${this.storageKeyPrefix}${this.userId}`);
      return storedCart ? JSON.parse(storedCart) as CartResponse : null;
    } catch {
      return null;
    }
  }

  private removeSavedCart(): void {
    if (this.userId !== 0) {
      localStorage.removeItem(`${this.storageKeyPrefix}${this.userId}`);
    }
  }
}