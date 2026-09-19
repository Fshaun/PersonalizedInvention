import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { firstValueFrom, isObservable, Observable } from 'rxjs';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { Order } from '../../core/models/order.model';
import { environment } from '../../../environments/environment';

type CheckoutStep = 'loading' | 'payment' | 'processing' | 'success' | 'error';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  step: CheckoutStep = 'loading';
  errorMessage = '';

  order: Order | null = null;
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  paymentElement: StripePaymentElement | null = null;
  clientSecret: string | null = null;

  constructor(
    private orderService: OrderService,
    private paymentService: PaymentService,
    public router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    await this.initCheckout();
  }

  ngOnDestroy(): void {
    // Clean up Stripe element when component is destroyed
    this.paymentElement?.destroy();
  }

  async initCheckout(): Promise<void> {
    try {
      this.step = 'loading';
      this.errorMessage = '';

      // Step 1 — create the order from the cart
      this.order = await this.requestWithTimeout(
        this.orderService.checkout(),
        'Creating your order',
        15000
      );
      if (!this.order) throw new Error('Failed to create order.');

      // Step 2 — ask the API to create a Stripe PaymentIntent
      const intent = await this.requestWithTimeout(
        this.paymentService.createPaymentIntent(this.order.id, this.order.totalAmount),
        'Initialising Stripe payment',
        15000
      );
      if (!intent) throw new Error('Failed to initialise payment.');

      this.clientSecret = intent.clientSecret;

      // Step 3 — load Stripe.js with your publishable key
      this.stripe = await this.requestWithTimeout(
        loadStripe(environment.stripePublishableKey),
        'Loading Stripe',
        15000
      );
      if (!this.stripe) throw new Error('Stripe failed to load. Check your publishable key.');

      // Step 4 — create the Stripe Elements and mount the payment form
      this.elements = this.stripe.elements({
        clientSecret: this.clientSecret,
        appearance: {
          theme: 'stripe',
          variables: {
            colorPrimary: '#e94560',
            colorBackground: '#ffffff',
            colorText: '#1a1a2e',
            borderRadius: '8px'
          }
        }
      });

      const cardElement = this.elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#1a1a2e',
            '::placeholder': { color: '#aab7c4' }
          }
        }
      });

      setTimeout(() => {
        cardElement.mount('#stripe-payment-element');
        this.step = 'payment';
      }, 100);
    } catch (err: any) {
      const message = err?.message || 'The checkout could not be initialised.';
      this.errorMessage = message;
      this.step = 'error';
    }
  }

  async pay(): Promise<void> {
    if (!this.stripe || !this.elements || !this.order) return;

    this.step = 'processing';
    this.errorMessage = '';

    // Get the card element
    const cardElement = this.elements.getElement('card');
    if (!cardElement) {
      this.errorMessage = 'Card element not found.';
      this.step = 'payment';
      return;
    }

    try {
      if (!this.clientSecret) {
        this.errorMessage = 'Payment session is not ready. Please try again.';
        this.step = 'payment';
        return;
      }

      const { error, paymentIntent } = await this.stripe.confirmCardPayment(
        this.clientSecret,
        { payment_method: { card: cardElement } }
      );

      if (error) {
        this.errorMessage = error.message ?? 'Payment failed.';
        this.step = 'payment';
        return;
      }

      if (paymentIntent?.status === 'succeeded') {
        await this.requestWithTimeout(
          this.paymentService.confirmPayment(paymentIntent.id, this.order.id),
          'Confirming payment',
          15000
        );

        this.step = 'success';
        setTimeout(() => this.router.navigate(['/']), 3000);
      }
    } catch (err: any) {
      this.errorMessage = err?.message || 'Payment failed. Please try again.';
      this.step = 'payment';
    }
  }

  private async requestWithTimeout<T>(
    request: Promise<T> | Observable<T>,
    label: string,
    timeoutMs: number = 15000
  ): Promise<T> {
    const requestPromise = isObservable(request) ? firstValueFrom(request) : request;
    const timeoutPromise = new Promise<never>((_, reject) => {
      setTimeout(() => {
        reject(new Error(`${label} timed out. Check that the backend API is running.`));
      }, timeoutMs);
    });

    return await Promise.race([requestPromise, timeoutPromise]);
  }

  get orderTotal(): number {
    return this.order?.totalAmount ?? 0;
  }
}