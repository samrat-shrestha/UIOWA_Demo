import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  isAuthenticated = false;
  private isBrowser: boolean;

  constructor(
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  async ngOnInit(): Promise<void> {
    // Skip auth check on server
    if (!this.isBrowser) {
      return;
    }
    
    this.isAuthenticated = await this.authService.isAuthenticated();
    console.log('Login component - isAuthenticated:', this.isAuthenticated);
    
    if (this.isAuthenticated) {
      this.redirectToHome();
    }
  }

  login(): void {
    // Use real Okta authentication
    this.authService.login();
  }

  private redirectToHome(): void {
    let returnUrl = '/';
    
    if (this.isBrowser) {
      returnUrl = localStorage.getItem('returnUrl') || '/receipts';
      localStorage.removeItem('returnUrl');
    }
    
    console.log('Redirecting to:', returnUrl);
    this.router.navigateByUrl(returnUrl);
  }
}
