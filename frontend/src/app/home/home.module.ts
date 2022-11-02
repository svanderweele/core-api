import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Route, RouterModule } from '@angular/router';

import { HomePageComponent } from './home-page/home-page.component';
import { InputsModule } from '../inputs/inputs.module';

const ROUTES: Route[] = [
  {
    path: '',
    component: HomePageComponent,
  },
];

@NgModule({
  declarations: [HomePageComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    ReactiveFormsModule,
    InputsModule,
  ],
})
export class HomeModule {}
