import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, from, lastValueFrom, of } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { switchMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { isPlatformBrowser } from '@angular/common';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Skip auth token in server-side rendering
    if (!isPlatformBrowser(this.platformId)) {
      return next.handle(request);
    }
    
    // Only add the auth token for API requests to our backend
    if (!request.url.includes(environment.apiUrl)) {
      return next.handle(request);
    }

    // Convert the Promise to an Observable for better handling
    return from(this.authService.isAuthenticated()).pipe(
      switchMap(async (isAuthenticated) => {
        if (isAuthenticated) {
          const token = await this.authService.getAccessToken();
          if (token) {
            request = request.clone({
              setHeaders: {
                Authorization: `Bearer ${token}`
              }
            });
          }
        }
        return request;
      }),
      switchMap(req => next.handle(req))
    );
  }
}
