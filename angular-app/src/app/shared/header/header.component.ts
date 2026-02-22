import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';
import { SearchService } from '../../core/services/search.service';
import { CustomerDto, UserRole } from '../../core/models/auth.models';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, MatMenuModule, MatButtonModule, MatIconModule],
  template: `
    <header class="header">
      <nav class="nav-container">
        <div class="logo">
          <a routerLink="/">🥬 VeggyWorldShop</a>
        </div>

        <!-- Search Box (Centered) -->
        <div class="header-search-container" [class.search-active]="isSearchFocused() || searchText()">
          <div class="header-search-wrapper">
            <mat-icon class="search-icon">search</mat-icon>
            <input 
              type="text" 
              class="header-search-input"
              placeholder="Search vegetables, produce..."
              [(ngModel)]="searchText"
              (ngModelChange)="onSearchChange($event)"
              (focus)="onSearchFocus()"
              (blur)="onSearchBlur()"
            />
            <button 
              *ngIf="searchText()" 
              mat-icon-button 
              class="clear-search-btn"
              (click)="clearSearch()"
            >
              <mat-icon>close</mat-icon>
            </button>
          </div>
        </div>

        <div class="nav-auth-wrapper">
          <ul class="nav-links">
            <ng-container *ngIf="isAuthenticated && isAdmin">
              <li>
                <button mat-button [matMenuTriggerFor]="adminMenu" class="admin-menu-btn">
                  <mat-icon>admin_panel_settings</mat-icon>
                  <span>Manage</span>
                  <mat-icon class="dropdown-icon">arrow_drop_down</mat-icon>
                </button>
                <mat-menu #adminMenu="matMenu" class="compact-menu">
                  <button mat-menu-item (click)="navigateTo('/admin')">
                    <mat-icon>dashboard</mat-icon>
                    <span>Admin Dashboard</span>
                  </button>
                  <button mat-menu-item (click)="navigateTo('/admin/products')">
                    <mat-icon>inventory_2</mat-icon>
                    <span>Products</span>
                  </button>
                  <button mat-menu-item (click)="navigateTo('/admin/categories')">
                    <mat-icon>category</mat-icon>
                    <span>Categories</span>
                  </button>
                  <button mat-menu-item (click)="navigateTo('/admin/weight-types')">
                    <mat-icon>scale</mat-icon>
                    <span>Weight Types</span>
                  </button>
                  <button mat-menu-item (click)="navigateTo('/admin/manage-users')">
                    <mat-icon>people</mat-icon>
                    <span>Users</span>
                  </button>
                </mat-menu>
              </li>
            </ng-container>
          </ul>

          <div class="auth-section">
            <ng-container *ngIf="!isAuthenticated">
              <a routerLink="/login" class="btn btn-outline">Login</a>
              <a routerLink="/register" class="btn btn-primary">Register</a>
            </ng-container>

            <ng-container *ngIf="isAuthenticated && currentUser">
              <div class="user-menu">
                <div class="user-info">
                  <div class="user-avatar">{{ getInitials() }}</div>
                  <div class="user-details">
                    <span class="user-name">{{ currentUser.firstName }} {{ currentUser.lastName }}</span>
                    <span class="user-role">{{ currentUser.role === UserRole.Admin ? 'Admin' : 'Customer' }}</span>
                  </div>
                </div>
                <button class="btn btn-logout" (click)="logout()">Logout</button>
              </div>
            </ng-container>
          </div>
        </div>
      </nav>
    </header>
  `,
  styles: [`
    .header {
      background-color: #fff;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      width: 100%;
      z-index: 10000;
      min-height: 50px;
      max-height: 50px;
      height: 50px;
      transition: all 0.3s ease;
    }

    /* Enhanced header when search is active */
    .header:has(.search-active) {
      box-shadow: 0 4px 20px rgba(76, 175, 80, 0.2);
      background: linear-gradient(to bottom, #ffffff 0%, #f8fff9 100%);
      z-index: 10001;
    }

    .nav-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 20px;
      padding: 0.4rem 20px;
      max-width: 1600px;
      margin: 0 auto;
      height: 50px;
    }

    .logo {
      flex-shrink: 0;
    }

    /* Search Box - Centered */
    .header-search-container {
      flex: 1;
      max-width: 500px;
      margin: 0 auto;
      transition: all 0.3s ease;
    }

    /* Enhanced search container when active */
    .header-search-container.search-active {
      max-width: 600px;
      transform: scale(1.02);
    }

    .header-search-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      background: #f8f8f8;
      border-radius: 25px;
      padding: 2px 6px 2px 12px;
      transition: all 0.3s ease;
      border: 2px solid transparent;
    }

    .header-search-wrapper:hover,
    .header-search-wrapper:focus-within {
      background: white;
      border-color: #4CAF50;
      box-shadow: 0 2px 8px rgba(76, 175, 80, 0.15);
    }

    /* Enhanced styling when search is active */
    .search-active .header-search-wrapper {
      background: white;
      border-color: #4CAF50;
      box-shadow: 0 4px 16px rgba(76, 175, 80, 0.25);
    }

    .search-icon {
      color: #4CAF50;
      margin-right: 8px;
      font-size: 20px;
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }

    .header-search-input {
      flex: 1;
      border: none;
      outline: none;
      font-size: 14px;
      padding: 8px 6px;
      background: transparent;
      color: #333;
      font-family: inherit;
      min-width: 0;
    }

    .header-search-input::placeholder {
      color: #999;
    }

    .clear-search-btn {
      flex-shrink: 0;
      color: #999 !important;
      width: 32px !important;
      height: 32px !important;
    }

    .clear-search-btn:hover {
      color: #4CAF50 !important;
      background: rgba(76, 175, 80, 0.1) !important;
    }

    /* Nav and Auth Section Wrapper */
    .nav-auth-wrapper {
      flex-shrink: 0;
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .logo a {
      font-size: 1rem;
      font-weight: bold;
      color: #4caf50;
      text-decoration: none;
      display: flex;
      align-items: center;
      gap: 0.3rem;
      line-height: 1;
    }

    .nav-links {
      display: flex;
      list-style: none;
      gap: 1rem;
      margin: 0;
      padding: 0;
      align-items: center;
    }

    .nav-links a {
      color: #333;
      text-decoration: none;
      font-weight: 500;
      font-size: 0.8rem;
      transition: color 0.3s;
      padding: 0.2rem 0;
      border-bottom: 2px solid transparent;
      line-height: 1;
    }

    .nav-links a:hover {
      color: #4caf50;
    }

    .nav-links a.active {
      color: #4caf50;
      border-bottom-color: #4caf50;
    }

    .admin-menu-btn {
      color: #333 !important;
      font-size: 0.8rem !important;
      font-weight: 500 !important;
      padding: 0.2rem 0.5rem !important;
      min-height: auto !important;
      height: auto !important;
      line-height: 1 !important;
      display: flex !important;
      align-items: center !important;
      gap: 0.2rem !important;
      text-transform: none !important;
      border-radius: 4px !important;
      transition: all 0.3s !important;
    }

    .admin-menu-btn:hover {
      background-color: rgba(76, 175, 80, 0.1) !important;
      color: #4caf50 !important;
    }

    .admin-menu-btn mat-icon {
      font-size: 16px !important;
      width: 16px !important;
      height: 16px !important;
      line-height: 1 !important;
    }

    .admin-menu-btn .dropdown-icon {
      font-size: 18px !important;
      width: 18px !important;
      height: 18px !important;
      margin-left: -0.2rem !important;
    }

    .auth-section {
      display: flex;
      align-items: center;
      gap: 0.6rem;
    }

    .btn {
      padding: 0.3rem 0.8rem;
      border-radius: 5px;
      font-weight: 500;
      text-decoration: none;
      border: none;
      cursor: pointer;
      transition: all 0.3s;
      font-size: 0.75rem;
      display: inline-block;
      line-height: 1;
    }

    .btn-outline {
      background-color: transparent;
      color: #4caf50;
      border: 2px solid #4caf50;
    }

    .btn-outline:hover {
      background-color: #4caf50;
      color: white;
    }

    .btn-primary {
      background-color: #4caf50;
      color: white;
    }

    .btn-primary:hover {
      background-color: #45a049;
    }

    .btn-logout {
      background-color: #f44336;
      color: white;
      font-size: 0.7rem;
      padding: 0.3rem 0.8rem;
    }

    .btn-logout:hover {
      background-color: #da190b;
    }

    .user-menu {
      display: flex;
      align-items: center;
      gap: 0.6rem;
    }

    .user-info {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .user-avatar {
      width: 26px;
      height: 26px;
      border-radius: 50%;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
      font-size: 0.7rem;
    }

    .user-details {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: 0.1rem;
    }

    .user-name {
      font-weight: 600;
      color: #333;
      font-size: 0.75rem;
      line-height: 1;
    }

    .user-role {
      font-size: 0.65rem;
      color: #666;
      background-color: #f0f0f0;
      padding: 0.1rem 0.35rem;
      border-radius: 10px;
      line-height: 1;
    }

    /* Material Menu Overrides */
    ::ng-deep .compact-menu.mat-mdc-menu-panel {
      min-width: 200px !important;
      max-width: 220px !important;
      border-radius: 8px !important;
    }

    ::ng-deep .compact-menu .mat-mdc-menu-content {
      padding: 0.25rem 0 !important;
    }

    ::ng-deep .compact-menu .mat-mdc-menu-item {
      min-height: 36px !important;
      height: 36px !important;
      padding: 0 12px !important;
      font-size: 0.8rem !important;
      line-height: 1 !important;
    }

    ::ng-deep .compact-menu .mat-mdc-menu-item mat-icon {
      font-size: 18px !important;
      width: 18px !important;
      height: 18px !important;
      margin-right: 8px !important;
      color: #4caf50 !important;
    }

    ::ng-deep .compact-menu .mat-mdc-menu-item:hover {
      background-color: rgba(76, 175, 80, 0.1) !important;
    }

    ::ng-deep .compact-menu .mat-mdc-menu-item span {
      line-height: 1 !important;
    }

    @media (max-width: 768px) {
      .header {
        min-height: 48px;
        max-height: 48px;
        height: 48px;
      }

      .nav-container {
        padding: 0.3rem 1rem;
        height: 48px;
      }

      .nav-links {
        flex-wrap: wrap;
        gap: 0.75rem;
        justify-content: center;
      }

      .logo a {
        font-size: 0.9rem;
      }

      .btn {
        padding: 0.25rem 0.6rem;
        font-size: 0.7rem;
      }
    }
  `]
})
export class HeaderComponent implements OnInit {
  currentUser: CustomerDto | null = null;
  isAuthenticated = false;
  isAdmin = false;
  UserRole = UserRole;

  // Search functionality using SearchService
  isSearchFocused = signal<boolean>(false);
  
  constructor(
    public authService: AuthService,
    public searchService: SearchService,
    private router: Router
  ) {}

  // Expose searchText from service for template binding
  get searchText() {
    return this.searchService.searchText;
  }

  set searchText(value: any) {
    // This setter is needed for [(ngModel)] two-way binding
    if (typeof value === 'string') {
      this.searchService.setSearchText(value);
    }
  }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAuthenticated = !!user;
      this.isAdmin = user?.role === UserRole.Admin;
    });
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  logout(): void {
    if (confirm('Are you sure you want to logout?')) {
      this.authService.logout();
    }
  }

  getInitials(): string {
    if (!this.currentUser) return '';
    return `${this.currentUser.firstName[0]}${this.currentUser.lastName[0]}`.toUpperCase();
  }

  /**
   * Handle search input focus
   */
  onSearchFocus(): void {
    this.isSearchFocused.set(true);
    this.searchService.notifySearchFocused();
  }

  /**
   * Handle search input blur
   */
  onSearchBlur(): void {
    setTimeout(() => {
      // Only hide if there's no search text
      if (!this.searchService.searchText()) {
        this.isSearchFocused.set(false);
      }
    }, 100);
  }

  /**
   * Handle search text change
   */
  onSearchChange(value: string): void {
    this.searchService.setSearchText(value);
  }

  /**
   * Clear search
   */
  clearSearch(): void {
    this.searchService.clearSearch();
  }
}
