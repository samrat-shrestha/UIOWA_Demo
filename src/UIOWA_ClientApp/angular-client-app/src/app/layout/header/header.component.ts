import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  isAuthenticated = false;
  userName = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  async ngOnInit(): Promise<void> {
    // Initial check
    this.isAuthenticated = await this.authService.isAuthenticated();
    console.log('Header component - initial auth state:', this.isAuthenticated);
    
    if (this.isAuthenticated) {
      const user = await this.authService.getUserInfo();
      this.userName = user.name || user.email || 'User';
    }
    
    // Subscribe to auth state changes
    this.authService.authState$.subscribe(async (state) => {
      this.isAuthenticated = state?.isAuthenticated || false;
      console.log('Header component - auth state updated:', this.isAuthenticated);
      
      if (this.isAuthenticated) {
        try {
          const user = await this.authService.getUserInfo();
          this.userName = user.name || user.email || 'User';
        } catch (error) {
          console.error('Error getting user info:', error);
          this.userName = 'User';
        }
      }
    });
  }

  login(): void {
    this.authService.login();
  }

  async logout(): Promise<void> {
    await this.authService.logout();
    
    // Navigate to home page after logout
    this.router.navigate(['/']);
    
    console.log('User logged out, navigating to home page');
  }
}
