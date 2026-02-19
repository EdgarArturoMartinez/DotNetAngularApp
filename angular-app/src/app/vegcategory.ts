export interface VegCategory {
  idCategory: number;
  categoryName: string;
  description?: string;
  createdAt?: string;
  productCount?: number;
}

export interface VegTypeWeightBasic {
  idTypeWeight: number;
  name: string;
  abbreviationWeight: string;
}