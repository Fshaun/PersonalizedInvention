import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, AdminOrder } from '../../core/services/admin.service';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-orders.component.html',
  styleUrls: ['./admin-orders.component.scss']
})
export class AdminOrdersComponent implements OnInit {
  orders: AdminOrder[] = [];
  isLoading = true;
  errorMessage = '';
  successMessage = '';
  expandedOrderId: number | null = null;

  readonly statuses = [
    'Pending',
    'PaymentProcessing',
    'Paid',
    'Shipped',
    'Delivered',
    'Cancelled'
  ];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.adminService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isLoading = false;
      }
    });
  }

  toggleExpand(orderId: number): void {
    this.expandedOrderId = this.expandedOrderId === orderId ? null : orderId;
  }

  updateStatus(order: AdminOrder, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;
    this.clearMessages();

    this.adminService.updateOrderStatus(order.id, status).subscribe({
      next: () => {
        order.status = status;
        this.successMessage = `Order #${order.id} status updated to ${status}.`;
      },
      error: (err) => { this.errorMessage = err.message; }
    });
  }

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

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}