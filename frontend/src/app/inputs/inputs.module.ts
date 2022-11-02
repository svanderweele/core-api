import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { InputComponent } from './input/input.component';

@NgModule({
  declarations: [InputComponent],
  imports: [CommonModule, ReactiveFormsModule],
  exports: [InputComponent],
})
export class InputsModule {}
