import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { ReceiptsRoutingModule } from './receipts-routing.module';
import { ReceiptListComponent } from './receipt-list/receipt-list.component';
import { ReceiptFormComponent } from './receipt-form/receipt-form.component';
import { ReceiptDetailComponent } from './receipt-detail/receipt-detail.component';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ReceiptsRoutingModule,
    ReceiptListComponent,
    ReceiptFormComponent,
    ReceiptDetailComponent
  ]
})
export class ReceiptsModule { }
