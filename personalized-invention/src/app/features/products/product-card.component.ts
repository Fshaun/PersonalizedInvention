import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Product } from '../../core/models/product.model';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent {
  @Input() product!: Product;
  addedToCart = false;
  isAdding = false;

  constructor(private cartService: CartService, private router: Router) {}

  addToCart(): void {
    if (!this.product.isInStock || this.isAdding) return;
    this.isAdding = true;

    this.cartService.addItem(this.product.id, 1).subscribe({
      next: () => {
        this.addedToCart = true;
        this.isAdding = false;
        setTimeout(() => this.addedToCart = false, 2000);
      },
      error: () => { this.isAdding = false; }
    });
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
  }
}