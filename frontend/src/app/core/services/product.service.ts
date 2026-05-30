import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { Product } from '../models/product.model';
import { CreateProductRequest } from '../models/create-product-request.model';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = `${environment.apiUrl}/products`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/all`, { headers: this.authHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  getPaged(page: number, size: number): Observable<{ items: Product[], totalCount: number }> {
    return this.http.get<{ items: Product[], totalCount: number }>(
      `${this.baseUrl}?pageNumber=${page}&pageSize=${size}`,
      { headers: this.authHeaders() }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`, { headers: this.authHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request, { headers: this.authHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  update(id: number, request: CreateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${id}`, request, { headers: this.authHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`, { headers: this.authHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  private authHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else if (typeof error.error === 'string' && error.error.trim()) {
      errorMessage = error.error.trim();
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
