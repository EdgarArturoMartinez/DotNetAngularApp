import { Component, inject, signal } from '@angular/core';
import { Weatherforecast } from './weatherforecast';
import { CommonModule } from '@angular/common';
import { Menu } from './menu/menu';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [CommonModule, Menu, RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css']    
})
export class App {
  
}
