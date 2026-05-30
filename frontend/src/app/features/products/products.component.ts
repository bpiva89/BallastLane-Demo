import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ProductService } from '../../core/services/product.service';
import { AuthService } from '../../core/services/auth.service';
import { Product } from '../../core/models/product.model';
import { CreateProductRequest } from '../../core/models/create-product-request.model';
import { ProductFormComponent } from '../product-form/product-form.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
    MatSnackBarModule,
    MatPaginatorModule,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
  displayedColumns: string[] = ['id', 'name', 'description', 'price', 'stock', 'createdAt', 'actions'];
  products: Product[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  // Paging state
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  constructor(
    private productService: ProductService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.productService.getPaged(this.pageIndex + 1, this.pageSize).subscribe({
      next: (data) => {
        this.products = data.items;
        this.totalCount = data.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load products. Make sure the backend is running.';
        console.error(err);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      width: '480px',
      data: { product: null }
    });

    dialogRef.afterClosed().subscribe((request: CreateProductRequest | null) => {
      if (!request) return;

      this.productService.create(request).subscribe({
        next: () => {
          this.snackBar.open('Product created.', 'Close', { duration: 3000 });
          this.loadProducts();
        },
        error: (err) => {
          this.snackBar.open(err.message || 'Failed to create product.', 'Close', { duration: 4000 });
        }
      });
    });
  }

  openEditDialog(product: Product): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      width: '480px',
      data: { product }
    });

    dialogRef.afterClosed().subscribe((request: CreateProductRequest | null) => {
      if (!request) return;

      this.productService.update(product.id, request).subscribe({
        next: () => {
          this.snackBar.open('Product updated.', 'Close', { duration: 3000 });
          this.loadProducts();
        },
        error: (err) => {
          this.snackBar.open(err.message || 'Failed to update product.', 'Close', { duration: 4000 });
        }
      });
    });
  }

  deleteProduct(id: number): void {
    if (!confirm('Delete this product?')) return;

    this.productService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Product deleted.', 'Close', { duration: 3000 });
        this.loadProducts();
      },
      error: (err) => {
        this.snackBar.open(err.message || 'Failed to delete product.', 'Close', { duration: 4000 });
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  get username(): string | null {
    return this.authService.getUsername();
  }
}
