import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order, DeliveryAddress } from '../models/order.model';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  private get userId(): number {
    return this.authService.userId;
  }

  getUserOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/user/${this.userId}`);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`);
  }

  // ← Now sends delivery address with the checkout request
  checkout(address: DeliveryAddress): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/checkout/${this.userId}`, address);
  }
}