import { Input, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Route, RouterModule } from '@angular/router';

import { AuthPageComponent } from './auth-page/auth-page.component';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './sign-up-form/sign-up-form.component';
import { InputsModule } from '../inputs/inputs.module';
import { NotLoggedInGuard } from '../guards/NotLoggedInGuard';

const ROUTES: Route[] = [
  {
    path: '',
    component: AuthPageComponent,
    canActivate: [NotLoggedInGuard],
    children: [
      { path: 'register', component: SignUpFormComponent },
      { path: 'login', component: SignInFormComponent },
    ],
  },
];

@NgModule({
  declarations: [AuthPageComponent, SignInFormComponent, SignUpFormComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    ReactiveFormsModule,
    InputsModule,
  ],
})
export class AuthModule {}
