import { Routes } from '@angular/router';
import { ProductListComponent } from './features/products/product-list.component';
import { CartComponent } from './features/cart/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';

export const routes: Routes = [
  { path: '',         component: ProductListComponent },
  { path: 'cart',     component: CartComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: '**',       redirectTo: '' }
];