import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from './auth.models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  user$: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);

  /**
   *
   */
  constructor() {
    this.getUser();
  }

  setUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.user$.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.user$.next(null);
  }

  getUser(): Observable<User | null> {
    //TODO: Call the /me endpoint to confirm the token is still valid
    const userItem = localStorage.getItem('user');

    if (userItem) {
      this.user$.next(JSON.parse(userItem));
    }

    return this.user$.asObservable();
  }
}
