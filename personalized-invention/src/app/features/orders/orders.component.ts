import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../core/models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  isLoading = true;
  errorMessage = '';

  // Tracks which order's details are expanded
  expandedOrderId: number | null = null;

  constructor(private orderService: OrderService, private router: Router) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.orderService.getUserOrders().subscribe({
      next: (orders) => {
        // Newest first
        this.orders = orders.sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isLoading = false;
      }
    });
  }

  toggleDetails(orderId: number): void {
    this.expandedOrderId = this.expandedOrderId === orderId ? null : orderId;
  }

  isExpanded(orderId: number): boolean {
    return this.expandedOrderId === orderId;
  }

  // Maps status string to a CSS class for the badge colour
  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending':           'badge--pending',
      'PaymentProcessing': 'badge--processing',
      'Paid':              'badge--paid',
      'Shipped':           'badge--shipped',
      'Delivered':         'badge--delivered',
      'Cancelled':         'badge--cancelled'
    };
    return map[status] ?? 'badge--pending';
  }

  goShopping(): void {
    this.router.navigate(['/']);
  }
}