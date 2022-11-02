import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Route, RouterModule } from '@angular/router';

import { CasinoPageComponent } from './casino-page/casino-page.component';
import { InputsModule } from '../inputs/inputs.module';
import { AuthGuard } from '../guards/AuthGuard';

const ROUTES: Route[] = [
  {
    path: '',
    component: CasinoPageComponent,
    canActivate: [AuthGuard],
    children: [],
  },
];

@NgModule({
  declarations: [CasinoPageComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    ReactiveFormsModule,
    InputsModule,
  ],
})
export class CasinoModule {}
