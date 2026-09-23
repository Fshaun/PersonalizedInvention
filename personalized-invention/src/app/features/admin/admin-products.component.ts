import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, AdminProduct, ProductFormData } from '../../core/services/admin.service';

type PanelMode = 'none' | 'create' | 'edit';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.scss']
})
export class AdminProductsComponent implements OnInit {
  products: AdminProduct[] = [];
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  // Form panel state
  panelMode: PanelMode = 'none';
  editingId: number | null = null;
  isSaving = false;

  form: ProductFormData = this.emptyForm();

  categories = ['Drinkware', 'Art', 'Accessories', 'Clothing', 'Stationery', 'Other'];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.adminService.getAllProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isLoading = false;
      }
    });
  }

  openCreate(): void {
    this.form = this.emptyForm();
    this.editingId = null;
    this.panelMode = 'create';
    this.clearMessages();
  }

  openEdit(product: AdminProduct): void {
    this.form = {
      name:        product.name,
      description: product.description,
      price:       product.price,
      stock:       product.stock,
      imageUrl:    product.imageUrl,
      category:    product.category
    };
    this.editingId = product.id;
    this.panelMode = 'edit';
    this.clearMessages();
  }

  closePanel(): void {
    this.panelMode = 'none';
    this.editingId = null;
    this.form = this.emptyForm();
  }

  saveProduct(): void {
    if (!this.form.name || !this.form.category || this.form.price <= 0) {
      this.errorMessage = 'Name, category and a valid price are required.';
      return;
    }

    this.isSaving = true;
    this.clearMessages();

    const request = this.panelMode === 'edit' && this.editingId
      ? this.adminService.updateProduct(this.editingId, this.form)
      : this.adminService.createProduct(this.form);

    request.subscribe({
      next: () => {
        this.successMessage = this.panelMode === 'edit'
          ? 'Product updated successfully.'
          : 'Product created successfully.';
        this.isSaving = false;
        this.closePanel();
        this.loadProducts();
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.isSaving = false;
      }
    });
  }

  deleteProduct(product: AdminProduct): void {
    if (!confirm(`Delete "${product.name}"? This cannot be undone.`)) return;

    this.adminService.deleteProduct(product.id).subscribe({
      next: () => {
        this.successMessage = `"${product.name}" deleted.`;
        this.loadProducts();
      },
      error: (err) => { this.errorMessage = err.message; }
    });
  }

  updateStock(product: AdminProduct, event: Event): void {
    const stock = parseInt((event.target as HTMLInputElement).value, 10);
    if (isNaN(stock) || stock < 0) return;

    this.adminService.updateStock(product.id, stock).subscribe({
      next: () => {
        product.stock = stock;
        product.isInStock = stock > 0;
        this.successMessage = `Stock updated for "${product.name}".`;
      },
      error: (err) => { this.errorMessage = err.message; }
    });
  }

  private emptyForm(): ProductFormData {
    return { name: '', description: '', price: 0, stock: 0, imageUrl: '', category: '' };
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}