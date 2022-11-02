import { Injectable } from '@angular/core';
import { CanActivate, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { UserService } from '../auth/user.service';

@Injectable()
export class NotLoggedInGuard implements CanActivate {
  constructor(private userService: UserService) {}

  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    return this.userService.getUser().pipe(map((e) => e === null));
  }
}
