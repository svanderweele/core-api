import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
} from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ErrorMessage } from 'src/app/globals';
import { AuthService } from '../auth.service';
import { UserService } from '../user.service';

@UntilDestroy()
@Component({
  selector: 'app-sign-in-form',
  templateUrl: './sign-in-form.component.html',
})
export class SignInFormComponent {
  errors: ErrorMessage[] = [];
  form: FormGroup;
  isLoading = false;

  constructor(
    fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private router: Router
  ) {
    this.form = fb.group({
      email: [
        'vanderweelesimon@gmail.com',
        [Validators.required, Validators.email],
      ],
      password: ['Test123', [Validators.required]],
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
      .login(this.form.value)
      .pipe(
        untilDestroyed(this),
        catchError((err: HttpErrorResponse) => {
          if (err.error.code === 'AUTH_INVALID_CREDENTIALS') {
            this.errors.push({ key: 'errors.invalid_credentials' });
          }
          this.isLoading = false;
          return of();
        })
      )
      .subscribe((response) => {
        this.isLoading = false;

        //TODO: Handle subscriptions
        this.authService
          .getUserByToken(response.token)
          .pipe(untilDestroyed(this))
          .subscribe((val) => {
            this.userService.setUser(val);

            this.router.navigate(['casino']);
          });
      });
  }
}
