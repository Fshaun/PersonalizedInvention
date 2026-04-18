import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { loadStripe, Stripe, StripeElements } from '@stripe/stripe-js';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  orderId: number | null = null;
  orderTotal = 0;
  isLoading = true;
  isProcessing = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private orderService: OrderService,
    private paymentService: PaymentService,
    public router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    await this.initCheckout();
  }

  async initCheckout(): Promise<void> {
    try {
      const order = await this.orderService.checkout().toPromise();
      if (!order) throw new Error('Failed to create order.');

      this.orderId = order.id;
      this.orderTotal = order.totalAmount;

      const intent = await this.paymentService
        .createPaymentIntent(order.id, order.totalAmount)
        .toPromise();

      if (!intent) throw new Error('Failed to initialize payment.');

      this.stripe = await loadStripe(environment.stripePublishableKey);
      if (!this.stripe) throw new Error('Stripe failed to load.');

      this.elements = this.stripe.elements({ clientSecret: intent.clientSecret });
      const paymentElement = this.elements.create('payment');
      paymentElement.mount('#payment-element');

      this.isLoading = false;
    } catch (err: any) {
      this.errorMessage = err.message;
      this.isLoading = false;
    }
  }

  async confirmPayment(): Promise<void> {
    if (!this.stripe || !this.elements) return;
    this.isProcessing = true;
    this.errorMessage = '';

    const { error } = await this.stripe.confirmPayment({
      elements: this.elements,
      confirmParams: { return_url: `${window.location.origin}/order-success` },
      redirect: 'if_required'
    });

    if (error) {
      this.errorMessage = error.message || 'Payment failed.';
      this.isProcessing = false;
    } else {
      this.successMessage = '🎉 Payment successful! Your order is confirmed.';
      setTimeout(() => this.router.navigate(['/']), 3000);
    }
  }
}