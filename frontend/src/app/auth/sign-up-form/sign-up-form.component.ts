import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { catchError, of } from 'rxjs';
import { ErrorMessage, ErrorResponse } from 'src/app/globals';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-sign-up-form',
  templateUrl: './sign-up-form.component.html',
  styleUrls: ['./sign-up-form.component.css'],
})
export class SignUpFormComponent {
  form: FormGroup;
  errors: ErrorMessage[] = [];
  isLoading = false;

  constructor(fb: FormBuilder, private authService: AuthService) {
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
        catchError((err: HttpErrorResponse, caught) => {
          if (err.error.code === 'AUTH_EMAIL_EXISTS') {
            this.errors.push({ key: 'errors.invalid_credentials' });
          }
          this.isLoading = false;
          return of();
        })
      )
      .subscribe((response) => {
        this.isLoading = false;
      });
  }
}
