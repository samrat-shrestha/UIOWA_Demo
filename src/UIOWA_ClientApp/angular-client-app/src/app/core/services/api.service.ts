import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;
  
  // Default headers with API key for simple authentication
  private defaultHeaders = new HttpHeaders({
    'X-API-Key': 'demo-key'
  });

  constructor(private http: HttpClient) { }

  get<T>(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${path}`, { 
      params,
      headers: this.defaultHeaders
    })
      .pipe(catchError(this.handleError));
  }

  post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${path}`, body, {
      headers: this.defaultHeaders
    })
      .pipe(catchError(this.handleError));
  }

  put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${path}`, body, {
      headers: this.defaultHeaders
    })
      .pipe(catchError(this.handleError));
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${path}`, {
      headers: this.defaultHeaders
    })
      .pipe(catchError(this.handleError));
  }

  getBlob(path: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${path}`, {
      headers: this.defaultHeaders,
      responseType: 'blob'
    })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('API error', error);
    return throwError(() => error);
  }
}
