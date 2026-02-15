import { Routes } from '@angular/router';
import { Landing } from './landing/landing';
import { IndexProducts } from './index-products/index-products';
import { CreateVegproduct } from './create-vegproduct/create-vegproduct';
import { EditVegproduct } from './edit-vegproduct/edit-vegproduct';

export const routes: Routes = [
    {path: '', component: Landing},
    {path: 'products', component: IndexProducts},
    {path: 'products/create', component: CreateVegproduct},
    {path: 'products/edit/:id', component: EditVegproduct}
];
