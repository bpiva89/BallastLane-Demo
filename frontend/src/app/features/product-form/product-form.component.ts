import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Product } from '../../core/models/product.model';
import { CreateProductRequest } from '../../core/models/create-product-request.model';

interface DialogData {
  product: Product | null;
}

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule
  ],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  name = '';
  description = '';
  price: number | null = null;
  stock: number | null = null;

  get isEdit(): boolean {
    return !!this.data.product;
  }

  constructor(
    private dialogRef: MatDialogRef<ProductFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  ngOnInit(): void {
    if (this.data.product) {
      this.name = this.data.product.name;
      this.description = this.data.product.description;
      this.price = this.data.product.price;
      this.stock = this.data.product.stock;
    }
  }

  onSave(): void {
    const request: CreateProductRequest = {
      name: this.name,
      description: this.description,
      price: this.price!,
      stock: this.stock!
    };
    this.dialogRef.close(request);
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
