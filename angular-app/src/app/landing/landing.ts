import { Component, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { Weatherforecast } from '../weatherforecast';


@Component({
  selector: 'app-landing',
  imports: [CommonModule, DatePipe, MatButtonModule, MatIconModule, MatProgressSpinnerModule, RouterLink],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  weatherForecastService = inject(Weatherforecast);
  weathers = signal<any[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    console.log('Component constructor called');
    console.log('Service:', this.weatherForecastService);
    
    const observable = this.weatherForecastService.getWeatherForecast();
    console.log('Observable created:', observable);
    
    observable.subscribe({
      next: (result) => {
        console.log('✓ SUCCESS - Received data:', result);
        console.log('Is array?', Array.isArray(result));
        const weatherData = Array.isArray(result) ? result : [];
        console.log('Weathers length:', weatherData.length);
        this.weathers.set(weatherData);
        this.isLoading.set(false);
        console.log('Updated signals - weathers:', this.weathers(), 'isLoading:', this.isLoading());
      },
      error: (err) => {
        console.error('✗ ERROR - Failed to fetch:', err);
        console.error('Error details:', err.message, err.status);
        this.error.set(`Failed to load weather data: ${err.message}. Check if API is running on https://localhost:7020`);
        this.isLoading.set(false);
      },
      complete: () => {
        console.log('Observable completed');
      }
    });
    
    console.log('Subscription created');
  }
}
