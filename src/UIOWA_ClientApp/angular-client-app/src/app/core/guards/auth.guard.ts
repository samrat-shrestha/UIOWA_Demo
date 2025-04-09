import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable, from } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { map, tap } from 'rxjs/operators';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  private isBrowser: boolean;

  constructor(
    private authService: AuthService, 
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    // Always allow navigation on the server to prevent blocking SSR
    if (!this.isBrowser) {
      return true;
    }
    
    console.log('Auth guard checking if user is authenticated...');
    
    return from(this.authService.isAuthenticated()).pipe(
      map(authenticated => {
        console.log('Is authenticated:', authenticated);
        
        if (!authenticated) {
          // Store the attempted URL for redirecting
          if (this.isBrowser) {
            console.log('Storing return URL:', state.url);
            localStorage.setItem('returnUrl', state.url);
          }
          
          // Redirect to login page
          console.log('Redirecting to login page');
          this.router.navigate(['/login']);
          return false;
        }
        
        return true;
      })
    );
  }
}
