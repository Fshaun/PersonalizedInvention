import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';
import { OrderService } from '../../core/services/order.service';
import { PaymentService } from '../../core/services/payment.service';
import { AuthService } from '../../core/services/auth.service';
import { DeliveryAddress, Order } from '../../core/models/order.model';
import { environment } from '../../../environments/environment';

type CheckoutStep = 'address' | 'payment' | 'processing' | 'success' | 'error';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  step: CheckoutStep = 'address';
  errorMessage = '';

  order: Order | null = null;
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  cardElement: StripeCardElement | null = null;

  // SA provinces for the dropdown
  readonly provinces = [
    'Gauteng', 'Western Cape', 'KwaZulu-Natal', 'Eastern Cape',
    'Limpopo', 'Mpumalanga', 'North West', 'Free State', 'Northern Cape'
  ];

  // Delivery address form — pre-fill name from logged-in user
  address: DeliveryAddress = {
    fullName:   '',
    phone:      '',
    street:     '',
    city:       '',
    province:   '',
    postalCode: '',
    country:    'South Africa'
  };

  constructor(
    private orderService: OrderService,
    private paymentService: PaymentService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit(): void {
    // Pre-fill name from the logged-in user's profile
    const user = this.authService.currentUser;
    if (user) {
      this.address.fullName = user.fullName;
    }
  }

  ngOnDestroy(): void {
    this.cardElement?.destroy();
  }

  // ── Step 1: Validate address and move to payment ──────────────
  async proceedToPayment(): Promise<void> {
    if (!this.validateAddress()) return;

    this.step = 'payment' as any;
    this.errorMessage = '';

    try {
      // Create the order with the delivery address
      this.order = await this.orderService
        .checkout(this.address)
        .toPromise() ?? null;

      if (!this.order) throw new Error('Failed to create order.');

      // Create Stripe PaymentIntent
      const intent = await this.paymentService
        .createPaymentIntent(this.order.id, this.order.totalAmount)
        .toPromise();

      if (!intent) throw new Error('Failed to initialise payment.');

      // Load Stripe and mount card element
      this.stripe = await loadStripe(environment.stripePublishableKey);
      if (!this.stripe) throw new Error('Stripe failed to load.');

      this.elements = this.stripe.elements();
      this.cardElement = this.elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#1a1a2e',
            fontFamily: 'Segoe UI, sans-serif',
            '::placeholder': { color: '#aab7c4' }
          },
          invalid: { color: '#e94560' }
        }
      });

      setTimeout(() => {
        this.cardElement?.mount('#stripe-card-element');
      }, 100);

    } catch (err: any) {
      this.errorMessage = err.message;
      this.step = 'error';
    }
  }

  // ── Step 2: Confirm payment ────────────────────────────────────
  async pay(): Promise<void> {
    if (!this.stripe || !this.cardElement || !this.order) return;

    this.step = 'processing';
    this.errorMessage = '';

    const intent = await this.paymentService
      .createPaymentIntent(this.order.id, this.order.totalAmount)
      .toPromise();

    if (!intent) {
      this.errorMessage = 'Could not initialise payment.';
      this.step = 'payment';
      return;
    }

    const { error, paymentIntent } = await this.stripe.confirmCardPayment(
      intent.clientSecret,
      { payment_method: { card: this.cardElement } }
    );

    if (error) {
      this.errorMessage = error.message ?? 'Payment failed. Please try again.';
      this.step = 'payment';
      return;
    }

    if (paymentIntent?.status === 'succeeded') {
      await this.paymentService
        .confirmPayment(paymentIntent.id, this.order.id)
        .toPromise();

      this.step = 'success';
      setTimeout(() => this.router.navigate(['/orders']), 3000);
    }
  }

  goBackToAddress(): void {
    this.cardElement?.destroy();
    this.cardElement = null;
    this.step = 'address';
  }

  get orderTotal(): number {
    return this.order?.totalAmount ?? 0;
  }

  private validateAddress(): boolean {
    const { fullName, phone, street, city, province, postalCode } = this.address;
    if (!fullName || !phone || !street || !city || !province || !postalCode) {
      this.errorMessage = 'Please fill in all delivery fields.';
      return false;
    }
    if (!/^\d{10}$/.test(phone.replace(/\s/g, ''))) {
      this.errorMessage = 'Please enter a valid 10-digit phone number.';
      return false;
    }
    this.errorMessage = '';
    return true;
  }
}