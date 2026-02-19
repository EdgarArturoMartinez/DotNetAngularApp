import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { VegTypeWeightService } from '../shared/services/veg-type-weight.service';
import { VegTypeWeightCreateUpdateDto } from '../shared/models/entities';
import { NotificationService } from '../shared/services/notification.service';

@Component({
  selector: 'app-create-vegtypeweight',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSlideToggleModule, RouterLink, CommonModule, MatIconModule],
  templateUrl: './create-vegtypeweight.html',
  styleUrl: './create-vegtypeweight.css',
})
export class CreateVegtypeweight {
  private readonly formBuilder = inject(FormBuilder);
  private typeWeightService = inject(VegTypeWeightService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);

  vegTypeWeightForm = this.formBuilder.group({
    name: ['', Validators.required],
    abbreviationWeight: ['', Validators.required],
    description: [''],
    isActive: [true]
  });

  saveChanges() {
    if (this.vegTypeWeightForm.valid) {
      const formValue = this.vegTypeWeightForm.value;
      const typeWeightData: VegTypeWeightCreateUpdateDto = {
        name: formValue.name!,
        abbreviationWeight: formValue.abbreviationWeight!,
        description: formValue.description || undefined,
        isActive: formValue.isActive ?? true
      };
      
      this.typeWeightService.create(typeWeightData).subscribe({
        next: (response) => {
          const name = formValue.name ?? undefined;
          this.notificationService.created('Weight Type', name);
          
          setTimeout(() => {
            this.router.navigate(['/admin/weight-types']);
          }, 1000);
        },
        error: (error) => {
          console.error('Full error object:', error);
          let errorMessage = 'Unknown error';
          
          if (error.status === 0) {
            errorMessage = 'Cannot connect to backend API. Make sure the .NET server is running on https://localhost:7020';
          } else if (error.error) {
            if (typeof error.error === 'string') {
              errorMessage = error.error;
            } else if (error.error.message) {
              errorMessage = error.error.message;
            } else if (error.error.title) {
              errorMessage = error.error.title;
            } else {
              errorMessage = JSON.stringify(error.error);
            }
          } else if (error.message) {
            errorMessage = error.message;
          }
          
          this.notificationService.saveError('create', errorMessage);
        }
      });
    } else {
      this.notificationService.validationError();
    }
  }
}
