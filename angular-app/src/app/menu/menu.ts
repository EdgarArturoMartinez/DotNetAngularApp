import { Component } from '@angular/core';
import { AdminHeaderComponent } from '../shared/components/admin-header/admin-header.component';

@Component({
  selector: 'app-menu',
  imports: [AdminHeaderComponent],
  templateUrl: './menu.html',
  styleUrl: './menu.css',
})
export class Menu {}
