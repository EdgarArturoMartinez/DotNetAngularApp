import { Injectable, inject } from '@angular/core';
import { Vegproduct } from './vegproduct';
import { VegCategoryService } from './vegcategory.service';
import { CustomerService } from './core/services/customer.service';
import { VegTypeWeightService } from './shared/services/veg-type-weight.service';
import { Observable, forkJoin, map } from 'rxjs';

export interface DashboardStats {
  totalCategories: number;
  totalProducts: number;
  totalStockQuantity: number;
  totalInventoryValue: number;
  totalUsers: number;
  totalWeightTypes: number;
  categoryBreakdown: CategoryStat[];
  topProducts: ProductStat[];
  lowStockProducts: ProductStat[];
}

export interface CategoryStat {
  categoryName: string;
  productCount: number;
  totalValue: number;
}

export interface ProductStat {
  id: number;
  name: string;
  price: number;
  stockQuantity: number;
  categoryName?: string;
  totalValue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private vegProductService = inject(Vegproduct);
  private vegCategoryService = inject(VegCategoryService);
  private customerService = inject(CustomerService);
  private vegTypeWeightService = inject(VegTypeWeightService);

  /**
   * Get comprehensive dashboard statistics
   * Includes: categories count, products count, stock value, and detailed breakdowns
   */
  getDashboardStats(): Observable<DashboardStats> {
    return forkJoin({
      products: this.vegProductService.getVegproducts(),
      categories: this.vegCategoryService.getVegcategories(),
      users: this.customerService.getAllCustomers(),
      weightTypes: this.vegTypeWeightService.getAll()
    }).pipe(
      map(({ products, categories, users, weightTypes }) => {
        // Ensure products is an array
        const productList = Array.isArray(products) ? products : [];
        const categoryList = Array.isArray(categories) ? categories : [];
        const userList = Array.isArray(users) ? users : [];
        const weightTypeList = Array.isArray(weightTypes) ? weightTypes : [];

        // Calculate totals
        const totalCategories = categoryList.length;
        const totalProducts = productList.length;
        const totalStockQuantity = productList.reduce(
          (sum, p) => sum + (p.stockQuantity || 0),
          0
        );
        const totalInventoryValue = productList.reduce(
          (sum, p) => sum + (p.price * (p.stockQuantity || 0)),
          0
        );

        // Category breakdown
        const categoryMap = new Map<string, CategoryStat>();
        productList.forEach(product => {
          const catName = product.vegCategory?.categoryName || 'Uncategorized';
          if (!categoryMap.has(catName)) {
            categoryMap.set(catName, {
              categoryName: catName,
              productCount: 0,
              totalValue: 0
            });
          }
          const stat = categoryMap.get(catName)!;
          stat.productCount++;
          stat.totalValue += product.price * (product.stockQuantity || 0);
        });

        // Top 5 most valuable products
        const topProducts = productList
          .map(p => ({
            id: p.id,
            name: p.name,
            price: p.price,
            stockQuantity: p.stockQuantity || 0,
            categoryName: p.vegCategory?.categoryName,
            totalValue: p.price * (p.stockQuantity || 0)
          }))
          .sort((a, b) => b.totalValue - a.totalValue)
          .slice(0, 5);

        // Low stock products (quantity <= 10)
        const lowStockProducts = productList
          .map(p => ({
            id: p.id,
            name: p.name,
            price: p.price,
            stockQuantity: p.stockQuantity || 0,
            categoryName: p.vegCategory?.categoryName,
            totalValue: p.price * (p.stockQuantity || 0)
          }))
          .filter(p => p.stockQuantity <= 10)
          .sort((a, b) => a.stockQuantity - b.stockQuantity)
          .slice(0, 5);

        return {
          totalCategories,
          totalProducts,
          totalStockQuantity,
          totalInventoryValue,
          totalUsers: userList.length,
          totalWeightTypes: weightTypeList.length,
          categoryBreakdown: Array.from(categoryMap.values()),
          topProducts,
          lowStockProducts
        };
      })
    );
  }
}
