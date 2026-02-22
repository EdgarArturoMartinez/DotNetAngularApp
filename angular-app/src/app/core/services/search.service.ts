import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  // Shared search text signal
  searchText = signal<string>('');
  
  // Events for search interactions
  searchFocused$ = new Subject<void>();
  searchChanged$ = new Subject<string>();

  /**
   * Update search text
   */
  setSearchText(value: string): void {
    this.searchText.set(value);
    this.searchChanged$.next(value);
  }

  /**
   * Notify that search was focused
   */
  notifySearchFocused(): void {
    this.searchFocused$.next();
  }

  /**
   * Clear search text
   */
  clearSearch(): void {
    this.searchText.set('');
    this.searchChanged$.next('');
  }
}
