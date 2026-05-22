import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from '../guards/auth.guard';
import { noAuthGuard } from '../guards/no-auth.guard';

const routes: Routes = [
  {
    path: 'admin',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./admin/admin.module').then((m) => m.AdminModule),
  },
  {
    path: 'dispatcher',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./dispatcher/dispatcher.module').then(
        (m) => m.DispatcherModule
      ),
  },
  {
    path: 'customer',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./customer/customer.module').then(
        (m) => m.CustomerModule
      ),
  },
  {
    path: 'home',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./home/home.module').then((m) => m.HomeModule),
  },
  {
    path: '',
    canActivate: [noAuthGuard],
    loadChildren: () =>
      import('./authentication/authentication.module').then(
        (m) => m.AuthenticationModule
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
