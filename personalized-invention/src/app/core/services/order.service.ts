import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
<<<<<<< HEAD
import { Order } from '../models/order.model';
=======
import { Order, DeliveryAddress } from '../models/order.model';
import { AuthService } from './auth.service';
>>>>>>> beta
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;
<<<<<<< HEAD
  private userId = 1;

  constructor(private http: HttpClient) {}
=======

  constructor(private http: HttpClient, private authService: AuthService) {}

  private get userId(): number {
    return this.authService.userId;
  }
>>>>>>> beta

  getUserOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/user/${this.userId}`);
  }

<<<<<<< HEAD
  checkout(): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/checkout/${this.userId}`, {});
=======
  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`);
  }

  // ← Now sends delivery address with the checkout request
  checkout(address: DeliveryAddress): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/checkout/${this.userId}`, address);
>>>>>>> beta
  }
}