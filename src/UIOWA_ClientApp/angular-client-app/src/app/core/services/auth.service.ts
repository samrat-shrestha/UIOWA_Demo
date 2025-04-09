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
  private readonly AUTH_STATE_KEY = 'app_auth_state';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    
    // Only initialize Okta on the browser
    if (this.isBrowser) {
      // Load cached auth state if available
      this.loadCachedAuthState();
      
      this.oktaAuth = new OktaAuth(environment.oktaConfig);
      this.oktaAuth.authStateManager.subscribe((authState) => {
        this.authStateSubject.next(authState);
        // Cache auth state to localStorage
        if (authState) {
          this.cacheAuthState(authState.isAuthenticated || false);
        }
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
        
        // Remove cached auth state
        localStorage.removeItem(this.AUTH_STATE_KEY);
      } catch (error) {
        console.error('Okta logout error:', error);
        
        // Even if signOut fails, clear local storage and update auth state
        this.clearOktaStorage();
        this.authStateSubject.next({ isAuthenticated: false } as AuthState);
        console.log('Auth state updated after logout (error case)');
        
        // Remove cached auth state
        localStorage.removeItem(this.AUTH_STATE_KEY);
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
    
    // For testing
    if (this._mockAuthState) {
      return true;
    }
    
    if (this.isBrowser) {
      try {
        // First check Okta
        const isAuth = await this.oktaAuth.isAuthenticated();
        console.log('Okta reports authentication status:', isAuth);
        
        // If authenticated, cache the state
        if (isAuth) {
          this.cacheAuthState(true);
        }
        
        return isAuth;
      } catch (error) {
        console.error('Error checking authentication status:', error);
        
        // Fall back to cached state if available
        const cachedState = this.getCachedAuthState();
        console.log('Falling back to cached auth state:', cachedState);
        return cachedState;
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
        return this.oktaAuth.handleLoginRedirect().then(() => {
          // Cache auth state after successful login
          this.cacheAuthState(true);
          return Promise.resolve();
        });
      } catch (error) {
        console.error('Error handling authentication callback:', error);
        return Promise.resolve();
      }
    }
    return Promise.resolve();
  }
  
  // Cache auth state in localStorage
  private cacheAuthState(isAuthenticated: boolean): void {
    if (this.isBrowser) {
      try {
        localStorage.setItem(this.AUTH_STATE_KEY, JSON.stringify({ isAuthenticated, timestamp: new Date().getTime() }));
        console.log('Auth state cached:', isAuthenticated);
      } catch (error) {
        console.error('Error caching auth state:', error);
      }
    }
  }
  
  // Get cached auth state from localStorage
  private getCachedAuthState(): boolean {
    if (this.isBrowser) {
      try {
        const cachedData = localStorage.getItem(this.AUTH_STATE_KEY);
        if (cachedData) {
          const data = JSON.parse(cachedData);
          // Consider cache valid for 24 hours (86400000 ms)
          const isValid = (new Date().getTime() - data.timestamp) < 86400000;
          return isValid && data.isAuthenticated;
        }
      } catch (error) {
        console.error('Error reading cached auth state:', error);
      }
    }
    return false;
  }
  
  // Load cached auth state on service initialization
  private loadCachedAuthState(): void {
    const isAuthenticated = this.getCachedAuthState();
    if (isAuthenticated) {
      this.authStateSubject.next({ isAuthenticated: true } as AuthState);
      console.log('Initialized with cached auth state: authenticated');
    } else {
      this.authStateSubject.next({ isAuthenticated: false } as AuthState);
      console.log('Initialized with cached auth state: not authenticated');
    }
  }
  
  // For testing only - mock the login state
  setMockAuthState(isAuthenticated: boolean): void {
    this._mockAuthState = isAuthenticated;
  }
}
