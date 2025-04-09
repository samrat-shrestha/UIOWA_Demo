import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
      },
      {
        path: 'receipts',
        loadChildren: () => import('./features/receipts/receipts.module').then(m => m.ReceiptsModule),
        canActivate: [AuthGuard]
      }
    ]
  }
];
