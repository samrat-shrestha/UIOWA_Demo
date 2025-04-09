import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { OktaAuth, AuthState } from '@okta/okta-auth-js';
import { Observable, from, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private oktaAuth!: OktaAuth;
  private authStateSubject = new BehaviorSubject<AuthState | null>(null);
  public authState$ = this.authStateSubject.asObservable();
  private isBrowser: boolean;
  
  // Add a flag to manually control authentication state for testing
  private _mockAuthState = false;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    
    // Only initialize Okta on the browser
    if (this.isBrowser) {
      this.oktaAuth = new OktaAuth(environment.oktaConfig);
      this.oktaAuth.authStateManager.subscribe((authState) => {
        this.authStateSubject.next(authState);
      });
    }
  }

  async login(): Promise<void> {
    if (this.isBrowser) {
      try {
        await this.oktaAuth.signInWithRedirect();
      } catch (error) {
        console.error('Okta login error:', error);
      }
    }
  }

  async logout(): Promise<void> {
    if (this.isBrowser) {
      try {
        // Clear Okta storage items
        this.clearOktaStorage();
        
        // Sign out from Okta
        await this.oktaAuth.signOut();
        
        // Explicitly update the auth state subject to notify subscribers
        this.authStateSubject.next({ isAuthenticated: false } as AuthState);
        console.log('Auth state updated after logout');
      } catch (error) {
        console.error('Okta logout error:', error);
        
        // Even if signOut fails, clear local storage and update auth state
        this.clearOktaStorage();
        this.authStateSubject.next({ isAuthenticated: false } as AuthState);
        console.log('Auth state updated after logout (error case)');
      }
    }
  }
  
  private clearOktaStorage(): void {
    if (this.isBrowser) {
      // Clear all Okta-related items from localStorage
      Object.keys(localStorage).forEach(key => {
        if (key.startsWith('okta-')) {
          console.log('Clearing localStorage item:', key);
          localStorage.removeItem(key);
        }
      });
      
      // Also clear any returnUrl that might have been set
      localStorage.removeItem('returnUrl');
    }
  }

  async isAuthenticated(): Promise<boolean> {
    console.log('Checking if user is authenticated...');
    
    if (this.isBrowser) {
      try {
        const isAuth = await this.oktaAuth.isAuthenticated();
        console.log('Okta reports authentication status:', isAuth);
        return isAuth;
      } catch (error) {
        console.error('Error checking authentication status:', error);
        return false;
      }
    }
    return false;
  }

  async getAccessToken(): Promise<string> {
    if (this.isBrowser) {
      try {
        const token = await this.oktaAuth.getAccessToken();
        return token || '';
      } catch (error) {
        console.error('Error getting access token:', error);
        return '';
      }
    }
    return '';
  }

  async getUserInfo(): Promise<any> {
    if (this.isBrowser) {
      try {
        const user = await this.oktaAuth.getUser();
        return user;
      } catch (error) {
        console.error('Error getting user info:', error);
        return {};
      }
    }
    return {};
  }

  handleAuthentication(): Promise<void> {
    if (this.isBrowser) {
      try {
        return this.oktaAuth.handleLoginRedirect();
      } catch (error) {
        console.error('Error handling authentication callback:', error);
        return Promise.resolve();
      }
    }
    return Promise.resolve();
  }
  
  // For testing only - mock the login state
  setMockAuthState(isAuthenticated: boolean): void {
    this._mockAuthState = isAuthenticated;
  }
}
