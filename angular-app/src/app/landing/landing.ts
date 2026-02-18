import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { RouterLink } from '@angular/router';
import { DashboardService, DashboardStats } from '../dashboard.service';


@Component({
  selector: 'app-landing',
  imports: [
    CommonModule,
    CurrencyPipe,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatDividerModule,
    MatTabsModule,
    MatListModule,
    RouterLink
  ],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing implements OnInit {
  dashboardService = inject(DashboardService);
  
  stats = signal<DashboardStats | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        console.log('✓ Dashboard stats loaded:', data);
        this.stats.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('✗ Error loading dashboard:', err);
        this.error.set(`Failed to load dashboard data: ${err.message}`);
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Refresh dashboard data
   */
  refreshDashboard() {
    this.isLoading.set(true);
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        console.log('✓ Dashboard refreshed:', data);
        this.stats.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('✗ Error refreshing dashboard:', err);
        this.error.set(`Failed to refresh dashboard: ${err.message}`);
        this.isLoading.set(false);
      }
    });
  }
}
