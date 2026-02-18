export interface ProductImage {
  id: number;
  idProduct: number;
  imageUrl: string;
  imageType: ProductImageType; // Main = 0, Mobile = 1, Gallery = 2
  displayOrder: number;
  width?: number;
  height?: number;
  uploadedDate: Date;
  isActive: boolean;
}

export interface ProductImageCreateUpdate {
  imageUrl: string;
  imageType: ProductImageType;
  displayOrder: number;
  width?: number;
  height?: number;
}

export enum ProductImageType {
  Main = 0,
  Mobile = 1,
  Gallery = 2
}

export interface ImageValidationResult {
  valid: boolean;
  message?: string;
}
