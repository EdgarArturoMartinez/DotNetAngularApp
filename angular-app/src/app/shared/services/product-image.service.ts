import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductImage, ProductImageCreateUpdate, ImageValidationResult } from '../models/product-image';

@Injectable({
  providedIn: 'root'
})
export class ProductImageService {
  private apiUrl = 'https://localhost:7020/api'; // Backend API running on port 7020

  constructor(private http: HttpClient) { }

  /**
   * Get all images for a specific product
   */
  getProductImages(productId: number): Observable<ProductImage[]> {
    return this.http.get<ProductImage[]>(`${this.apiUrl}/products/${productId}/images`);
  }

  /**
   * Get a specific image by ID
   */
  getImageById(productId: number, imageId: number): Observable<ProductImage> {
    return this.http.get<ProductImage>(`${this.apiUrl}/products/${productId}/images/${imageId}`);
  }

  /**
   * Get the main/hero image for a product
   */
  getMainImage(productId: number): Observable<ProductImage> {
    return this.http.get<ProductImage>(`${this.apiUrl}/products/${productId}/images/main`);
  }

  /**
   * Get the mobile optimized image for a product
   */
  getMobileImage(productId: number): Observable<ProductImage> {
    return this.http.get<ProductImage>(`${this.apiUrl}/products/${productId}/images/mobile`);
  }

  /**
   * Get gallery images for a product
   */
  getGalleryImages(productId: number): Observable<ProductImage[]> {
    return this.http.get<ProductImage[]>(`${this.apiUrl}/products/${productId}/images/gallery`);
  }

  /**
   * Create a new product image
   */
  createImage(productId: number, imageData: ProductImageCreateUpdate): Observable<ProductImage> {
    return this.http.post<ProductImage>(`${this.apiUrl}/products/${productId}/images`, imageData);
  }

  /**
   * Update an existing product image
   */
  updateImage(productId: number, imageId: number, imageData: ProductImageCreateUpdate): Observable<ProductImage> {
    return this.http.put<ProductImage>(`${this.apiUrl}/products/${productId}/images/${imageId}`, imageData);
  }

  /**
   * Delete a product image
   */
  deleteImage(productId: number, imageId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${productId}/images/${imageId}`);
  }

  /**
   * Delete all images for a product
   */
  deleteAllProductImages(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/products/${productId}/images`);
  }

  /**
   * Get image count for a product
   */
  getImageCount(productId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/products/${productId}/images/count`);
  }

  /**
   * Validate image dimensions and type before upload
   */
  validateImage(productId: number, imageData: ProductImageCreateUpdate): Observable<ImageValidationResult> {
    return this.http.post<ImageValidationResult>(`${this.apiUrl}/products/${productId}/images/validate`, imageData);
  }

  /**
   * Upload a product image file
   */
  uploadImageFile(productId: number, file: File, imageType: number, displayOrder: number = 0): Observable<ProductImage> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('imageType', imageType.toString());
    formData.append('displayOrder', displayOrder.toString());

    return this.http.post<ProductImage>(
      `${this.apiUrl}/products/${productId}/images/upload`,
      formData
    );
  }
}
