import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { VegTypeWeightService } from '../shared/services/veg-type-weight.service';
import { VegTypeWeight, VegTypeWeightCreateUpdateDto } from '../shared/models/entities';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-edit-vegtypeweight',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSlideToggleModule, RouterLink, CommonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './edit-vegtypeweight.html',
  styleUrl: './edit-vegtypeweight.css',
})
export class EditVegtypeweight implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private typeWeightService = inject(VegTypeWeightService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  typeWeightId: number = 0;
  isLoading = true;
  typeWeight: VegTypeWeight | null = null;

  vegTypeWeightForm = this.formBuilder.group({
    name: ['', Validators.required],
    abbreviationWeight: ['', Validators.required],
    description: [''],
    isActive: [true]
  });

  ngOnInit() {
    console.log('EditVegtypeweight ngOnInit called');
    const id = this.route.snapshot.paramMap.get('id');
    console.log('Weight Type ID from route:', id);
    if (id && id !== 'undefined' && id.trim() !== '') {
      this.typeWeightId = parseInt(id);
      console.log('Parsed typeWeightId:', this.typeWeightId);
      if (!isNaN(this.typeWeightId)) {
        this.loadTypeWeight();
      } else {
        console.log('Invalid weight type ID format:', id);
        this.notificationService.error('Invalid weight type ID');
        this.router.navigate(['/admin/weight-types']);
      }
    } else {
      this.notificationService.error('Invalid weight type ID');
      this.router.navigate(['/admin/weight-types']);
    }
  }

  loadTypeWeight() {
    console.log('loadTypeWeight called for ID:', this.typeWeightId);
    this.isLoading = true;
    console.log('Calling API:', `https://localhost:7020/api/vegtypeweights/${this.typeWeightId}`);
    
    this.typeWeightService.getById(this.typeWeightId).subscribe({
      next: (typeWeight) => {
        try {
          console.log('SUCCESS! Weight Type loaded:', typeWeight);
          this.typeWeight = typeWeight;
          
          this.vegTypeWeightForm.patchValue({
            name: typeWeight.name,
            abbreviationWeight: typeWeight.abbreviationWeight,
            description: typeWeight.description || '',
            isActive: typeWeight.isActive
          });
          console.log('Form patched successfully');
          
          this.isLoading = false;
          this.cdr.detectChanges();
          console.log('Change detection triggered!');
        } catch (error) {
          console.error('ERROR in success handler:', error);
          this.isLoading = false;
        }
      },
      error: (error) => {
        console.error('ERROR loading weight type:', error);
        
        let errorMsg = 'Error loading weight type';
        if (error.status === 404) {
          errorMsg = 'Weight type not found';
        } else if (error.status === 0) {
          errorMsg = 'Cannot connect to backend. Check if API is running and CORS is configured.';
        } else {
          errorMsg = `Error ${error.status}: ${error.message || 'Unknown error'}`;
        }
        
        this.notificationService.error(errorMsg, 6000);
        this.isLoading = false;
        setTimeout(() => {
          this.router.navigate(['/admin/weight-types']);
        }, 2000);
      }
    });
  }

  saveChanges() {
    if (this.vegTypeWeightForm.valid && this.typeWeightId) {
      const formValue = this.vegTypeWeightForm.value;
      const typeWeightData: VegTypeWeightCreateUpdateDto = {
        name: formValue.name!,
        abbreviationWeight: formValue.abbreviationWeight!,
        description: formValue.description || undefined,
        isActive: formValue.isActive ?? true
      };
      
      this.typeWeightService.update(this.typeWeightId, typeWeightData).subscribe({
        next: () => {
          const name = formValue.name ?? undefined;
          this.notificationService.updated('Weight Type', name);
          
          setTimeout(() => {
            this.router.navigate(['/admin/weight-types']);
          }, 1000);
        },
        error: (error) => {
          let errorMessage = 'Unknown error';
          if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            } else {
              errorMessage = JSON.stringify(error.error);
            }
          }
          
          this.notificationService.saveError('update', errorMessage);
        }
      });
    } else {
      this.notificationService.validationError();
    }
  }
}
