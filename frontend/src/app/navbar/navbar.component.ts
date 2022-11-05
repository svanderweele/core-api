import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { User } from '../auth/auth.models';
import { UserService } from '../auth/user.service';
import { Urls } from '../globals';

type Url = {
  path: Urls;
  key: string;
};

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  mainNavUrls: Url[] = [
    {
      path: '/',
      key: 'nav.home',
    },
    {
      path: '/casino',
      key: 'nav.casino',
    },
  ];

  secondaryNavUrls: Url[] = [
    {
      path: '/authentication/register',
      key: 'nav.register',
    },
    {
      path: '/authentication/login',
      key: 'nav.login',
    },
  ];

  user$: Observable<User | null>;

  constructor(public router: Router, private userService: UserService) {
    this.user$ = userService.getUser();
  }

  navigate(url: Urls): void {
    this.router.navigate([url]);
  }

  signOut(): void {
    this.userService.logout();
    this.router.navigate(['/']);
  }
}
