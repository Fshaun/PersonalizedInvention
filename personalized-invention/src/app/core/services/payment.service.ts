import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
}

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private apiUrl = `${environment.apiUrl}/payments`;

  constructor(private http: HttpClient) {}

  createPaymentIntent(orderId: number, amount: number): Observable<PaymentIntentResponse> {
    return this.http.post<PaymentIntentResponse>(`${this.apiUrl}/create-intent`, {
      orderId,
      amount,
      currency: 'zar'
    });
  }

  confirmPayment(paymentIntentId: string, orderId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/confirm`, { paymentIntentId, orderId });
  }
}