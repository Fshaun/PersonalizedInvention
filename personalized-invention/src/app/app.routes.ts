import { Routes } from '@angular/router';
import { ProductListComponent } from './features/products/product-list.component';
import { CartComponent } from './features/cart/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { OrdersComponent } from './features/orders/orders.component';
import { authGuard } from './core/guards/auth.guard';

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
    path: 'orders',             // ← new
    component: OrdersComponent,
    canActivate: [authGuard]    // ← must be logged in
  },
  { path: '**', redirectTo: '' }
];