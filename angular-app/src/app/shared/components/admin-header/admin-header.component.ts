import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { AuthService } from '../../../core/services/auth.service';
import { CustomerDto, UserRole } from '../../../core/models/auth.models';

interface NavigationItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

@Component({
  selector: 'app-admin-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    MatBadgeModule
  ],
  templateUrl: './admin-header.component.html',
  styleUrls: ['./admin-header.component.css']
})
export class AdminHeaderComponent implements OnInit {
  @Input() variant: 'full' | 'compact' = 'full';
  
  currentUser: CustomerDto | null = null;
  isAuthenticated = false;
  isAdmin = false;
  
  // Navigation items for entity management
  manageItems: NavigationItem[] = [
    { label: 'Products', icon: 'inventory_2', route: '/admin/products' },
    { label: 'Categories', icon: 'category', route: '/admin/categories' },
    { label: 'Weight Types', icon: 'straighten', route: '/admin/weight-types' },
    { label: 'Users', icon: 'people', route: '/admin/users' }
  ];

  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isAuthenticated = !!user;
      this.isAdmin = user?.role === UserRole.Admin;
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  getCurrentRoute(): string {
    return this.router.url;
  }

  isRouteActive(route: string): boolean {
    return this.router.url === route;
  }

  isManageMenuActive(): boolean {
    return this.manageItems.some(item => this.router.url.startsWith(item.route));
  }
}
