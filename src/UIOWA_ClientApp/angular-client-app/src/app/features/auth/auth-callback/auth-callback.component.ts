import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-auth-callback',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.scss']
})
export class AuthCallbackComponent implements OnInit {
  loading = true;
  error = '';
  private isBrowser: boolean;

  constructor(
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  async ngOnInit(): Promise<void> {
    // Skip handling on the server
    if (!this.isBrowser) {
      return;
    }
    
    try {
      await this.authService.handleAuthentication();
      
      // Get the saved redirect URL or default to home
      let returnUrl = '/';
      
      if (this.isBrowser) {
        returnUrl = localStorage.getItem('returnUrl') || '/';
        localStorage.removeItem('returnUrl');
      }
      
      // Navigate to the return URL
      this.router.navigateByUrl(returnUrl);
    } catch (error: any) {
      this.error = 'Failed to process authentication. Please try again.';
      console.error('Auth callback error:', error);
      this.loading = false;
    }
  }
}
