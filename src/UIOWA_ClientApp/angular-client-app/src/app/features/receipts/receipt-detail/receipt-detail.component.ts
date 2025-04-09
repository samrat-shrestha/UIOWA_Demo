import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReceiptService, Receipt } from '../services/receipt.service';

@Component({
  selector: 'app-receipt-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './receipt-detail.component.html',
  styleUrl: './receipt-detail.component.scss'
})
export class ReceiptDetailComponent implements OnInit {
  receipt: Receipt | null = null;
  loading = true;
  error = '';
  receiptId = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private receiptService: ReceiptService
  ) { }

  ngOnInit(): void {
    this.receiptId = this.route.snapshot.paramMap.get('id') || '';
    if (!this.receiptId) {
      this.error = 'Invalid receipt ID';
      this.loading = false;
      return;
    }

    this.loadReceipt();
  }

  loadReceipt(): void {
    this.loading = true;
    this.error = '';

    this.receiptService.getReceiptById(this.receiptId).subscribe({
      next: (data) => {
        this.receipt = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading receipt details', err);
        this.error = 'Failed to load receipt details. Please try again.';
        this.loading = false;
        
        // If 404 Not Found, show specific message
        if (err.status === 404) {
          this.error = 'Receipt not found';
        }
        
        // If 401 Unauthorized, redirect to login
        if (err.status === 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatDateTime(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', { 
      style: 'currency', 
      currency: 'USD' 
    }).format(amount);
  }

  goBack(): void {
    this.router.navigate(['/receipts']);
  }
  
  downloadReceipt(): void {
    if (!this.receipt) return;
    
    this.loading = true;
    this.receiptService.downloadReceipt(this.receipt.id).subscribe({
      next: (blob) => {
        this.loading = false;
        // Create a URL for the blob
        const url = window.URL.createObjectURL(blob);
        
        // Create a temporary link and click it to download
        const a = document.createElement('a');
        a.href = url;
        a.download = `receipt-${this.receipt!.id}${this.getFileExtensionFromBlob(blob)}`;
        document.body.appendChild(a);
        a.click();
        
        // Clean up
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      },
      error: (err) => {
        console.error('Error downloading receipt', err);
        this.loading = false;
        this.error = 'Failed to download receipt. Please try again.';
        
        // If 401 Unauthorized, redirect to login
        if (err.status === 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }
  
  private getFileExtensionFromBlob(blob: Blob): string {
    switch(blob.type) {
      case 'application/pdf':
        return '.pdf';
      case 'image/png':
        return '.png';
      case 'image/jpeg':
        return '.jpg';
      default:
        return '';
    }
  }
}
