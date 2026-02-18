import { Component, inject, signal } from '@angular/core';
import { Weatherforecast } from './weatherforecast';
import { CommonModule } from '@angular/common';
import { Menu } from './menu/menu';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  imports: [CommonModule, Menu, RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css']    
})
export class App {
  private router = inject(Router);
  
  // Signal to track if we're on an admin route
  isAdminRoute = signal(false);

  constructor() {
    // Check initial route
    this.checkRoute(this.router.url);

    // Listen to route changes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.checkRoute(event.urlAfterRedirects || event.url);
    });
  }

  /**
   * Check if current route is an admin route
   */
  private checkRoute(url: string): void {
    this.isAdminRoute.set(url.startsWith('/admin'));
  }
}
