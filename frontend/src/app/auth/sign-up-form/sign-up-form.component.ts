import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { catchError, of } from 'rxjs';
import { ErrorMessage } from 'src/app/globals';
import { AuthService } from '../auth.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { Router } from '@angular/router';

@UntilDestroy()
@Component({
  selector: 'app-sign-up-form',
  templateUrl: './sign-up-form.component.html',
  styleUrls: ['./sign-up-form.component.css'],
})
export class SignUpFormComponent {
  form: FormGroup;
  errors: ErrorMessage[] = [];
  isLoading = false;

  constructor(
    fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = fb.group({
      name: ['Simon', Validators.required],
      email: [
        'vanderweelesimon@gmail.com',
        [Validators.required, Validators.email],
      ],
      password: ['Test123', [Validators.required, Validators.minLength(5)]],
    });
  }

  getControl(id: string): FormControl {
    //TODO: Resolve this unsafe cast
    return this.form.get(id) as FormControl;
  }

  submitForm(): void {
    this.errors = [];
    this.isLoading = true;

    this.authService
      .registerUser(this.form.value)
      .pipe(
        untilDestroyed(this),
        catchError((err: HttpErrorResponse) => {
          if (err.error.code === 'AUTH_EMAIL_EXISTS') {
            this.errors.push({ key: 'errors.invalid_credentials' });
          }
          this.isLoading = false;
          return of();
        })
      )
      .subscribe(() => {
        this.isLoading = false;
        this.router.navigate(['login']);
      });
  }
}
