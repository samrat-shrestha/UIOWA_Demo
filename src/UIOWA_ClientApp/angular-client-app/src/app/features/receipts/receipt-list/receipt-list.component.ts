import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ReceiptService, Receipt } from '../services/receipt.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-receipt-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './receipt-list.component.html',
  styleUrls: ['./receipt-list.component.scss']
})
export class ReceiptListComponent implements OnInit {
  receipts: Receipt[] = [];
  loading = true;
  error = '';

  constructor(
    private receiptService: ReceiptService,
    private authService: AuthService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    // Double check authentication as a safeguard
    const isAuthenticated = await this.authService.isAuthenticated();
    if (!isAuthenticated) {
      console.log('User not authenticated, redirecting to login page');
      this.router.navigate(['/login']);
      return;
    }
    
    this.loadReceipts();
  }

  loadReceipts(): void {
    this.loading = true;
    this.receiptService.getReceipts().subscribe({
      next: (data) => {
        this.receipts = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading receipts', error);
        this.error = 'Failed to load receipts. Please try again.';
        this.loading = false;
        
        // If 401 Unauthorized, redirect to login
        if (error.status === 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', { 
      style: 'currency', 
      currency: 'USD' 
    }).format(amount);
  }
}
