import { Injectable } from '@angular/core';
import { CanActivate, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { UserService } from '../auth/user.service';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private userService: UserService,
    private toastrService: ToastrService
  ) {}

  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    return this.userService.getUser().pipe(
      map((e) => {
        if (e === null) {
          this.toastrService.error(
            'You must be logged in to access this page!'
          );
        }

        return e !== null;
      })
    );
  }
}
