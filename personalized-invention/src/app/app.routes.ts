import { Routes } from '@angular/router';
import { ProductListComponent } from './features/products/product-list.component';
import { CartComponent } from './features/cart/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { OrdersComponent } from './features/orders/orders.component';
import { AdminLayoutComponent } from './features/admin/admin-layout.component';
import { AdminProductsComponent } from './features/admin/admin-products.component';
import { AdminOrdersComponent } from './features/admin/admin-orders.component';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '',         component: ProductListComponent },
  { path: 'login',    component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'cart',
    component: CartComponent,
    canActivate: [authGuard]
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [authGuard]
  },
  {
    path: 'orders',
    component: OrdersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],          // ← only admins
    children: [
      { path: '',         redirectTo: 'products', pathMatch: 'full' },
      { path: 'products', component: AdminProductsComponent },
      { path: 'orders',   component: AdminOrdersComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];