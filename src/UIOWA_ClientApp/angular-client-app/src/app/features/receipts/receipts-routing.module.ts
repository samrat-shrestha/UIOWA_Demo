import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReceiptListComponent } from './receipt-list/receipt-list.component';
import { ReceiptFormComponent } from './receipt-form/receipt-form.component';
import { ReceiptDetailComponent } from './receipt-detail/receipt-detail.component';

const routes: Routes = [
  { path: '', component: ReceiptListComponent },
  { path: 'submit', component: ReceiptFormComponent },
  { path: ':id', component: ReceiptDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReceiptsRoutingModule { }
