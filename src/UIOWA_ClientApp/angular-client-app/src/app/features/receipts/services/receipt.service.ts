import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

export interface Receipt {
  id: string;
  purchaseDate: string;
  amount: number;
  description: string;
  receiptPath: string;
  createdUtc: string;
}

export interface SubmitReceiptDto {
  purchaseDate: string;
  amount: number;
  description: string;
  receiptFile: File;
}

@Injectable({
  providedIn: 'root'
})
export class ReceiptService {

  constructor(private apiService: ApiService) { }

  getReceipts(): Observable<Receipt[]> {
    return this.apiService.get<Receipt[]>('receipts');
  }

  getReceiptById(id: string): Observable<Receipt> {
    return this.apiService.get<Receipt>(`receipts/${id}`);
  }

  submitReceipt(receipt: SubmitReceiptDto): Observable<Receipt> {
    const formData = new FormData();
    formData.append('purchaseDate', receipt.purchaseDate);
    formData.append('amount', receipt.amount.toString());
    formData.append('description', receipt.description);
    formData.append('receiptFile', receipt.receiptFile);

    return this.apiService.post<Receipt>('receipts', formData);
  }
}
