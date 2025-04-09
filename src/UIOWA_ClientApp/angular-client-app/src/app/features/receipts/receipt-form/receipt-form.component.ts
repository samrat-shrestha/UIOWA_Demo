import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { ReceiptService, SubmitReceiptDto } from '../services/receipt.service';

@Component({
  selector: 'app-receipt-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatCardModule,
    MatProgressBarModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './receipt-form.component.html',
  styleUrl: './receipt-form.component.scss'
})
export class ReceiptFormComponent implements OnInit {
  receiptForm!: FormGroup;
  selectedFile: File | null = null;
  fileName: string = '';
  isSubmitting = false;
  maxFileSize = 5 * 1024 * 1024; // 5MB
  allowedFileTypes = ['application/pdf', 'image/png', 'image/jpeg'];

  constructor(
    private fb: FormBuilder,
    private receiptService: ReceiptService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.receiptForm = this.fb.group({
      purchaseDate: [new Date(), [Validators.required]],
      amount: ['', [Validators.required, Validators.min(0.01), Validators.max(99999.99)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    if (input.files && input.files.length) {
      const file = input.files[0];
      
      // Validate file size
      if (file.size > this.maxFileSize) {
        this.snackBar.open('File is too large. Maximum size is 5MB.', 'Close', { duration: 5000 });
        return;
      }
      
      // Validate file type
      if (!this.allowedFileTypes.includes(file.type)) {
        this.snackBar.open('Invalid file type. Allowed types: PDF, PNG, JPEG', 'Close', { duration: 5000 });
        return;
      }
      
      this.selectedFile = file;
      this.fileName = file.name;
    }
  }

  onSubmit(): void {
    if (this.receiptForm.invalid) {
      this.receiptForm.markAllAsTouched();
      return;
    }

    if (!this.selectedFile) {
      this.snackBar.open('Please select a receipt file', 'Close', { duration: 3000 });
      return;
    }

    this.isSubmitting = true;

    const formValue = this.receiptForm.value;
    const purchaseDate = new Date(formValue.purchaseDate);
    
    const receiptData: SubmitReceiptDto = {
      purchaseDate: `${purchaseDate.getFullYear()}-${String(purchaseDate.getMonth() + 1).padStart(2, '0')}-${String(purchaseDate.getDate()).padStart(2, '0')}`,
      amount: formValue.amount,
      description: formValue.description || '',
      receiptFile: this.selectedFile
    };

    this.receiptService.submitReceipt(receiptData).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        this.snackBar.open('Receipt submitted successfully!', 'Close', { duration: 3000 });
        this.router.navigate(['/receipts']);
      },
      error: (error) => {
        this.isSubmitting = false;
        console.error('Error submitting receipt:', error);
        this.snackBar.open('Failed to submit receipt. Please try again.', 'Close', { duration: 5000 });
      }
    });
  }
}
