import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { CartItem } from '../../core/models/cart-item.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],   // ✅ needed for number pipe
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  total = 0;
  isLoading = true;

  constructor(private cartService: CartService, private router: Router) {}

  ngOnInit(): void { this.loadCart(); }

  loadCart(): void {
    this.isLoading = true;
    this.cartService.getCart().subscribe({
      next: (res) => {
        this.cartItems = res.items;
        this.total = res.total;
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  updateQuantity(productId: number, quantity: number): void {
    this.cartService.updateItem(productId, quantity).subscribe({
      next: () => this.loadCart()
    });
  }

  removeItem(productId: number): void {
    this.cartService.removeItem(productId).subscribe({
      next: () => this.loadCart()
    });
  }

  clearCart(): void {
    this.cartService.clearCart().subscribe({
      next: () => this.loadCart()
    });
  }

  proceedToCheckout(): void { this.router.navigate(['/checkout']); }
  continueShopping(): void { this.router.navigate(['/']); }
}