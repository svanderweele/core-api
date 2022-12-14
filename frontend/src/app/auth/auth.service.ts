import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  GetUserResponse,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  User,
} from './auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  user$: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);

  constructor(private http: HttpClient) {}

  registerUser(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${environment.authApiUrl}/api/authentication/register`,
      request
    );
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(
      `${environment.authApiUrl}/api/authentication/login`,
      request
    );
  }

  getUserByToken(token: string): Observable<GetUserResponse> {
    return this.http.get<User>(
      `${environment.authApiUrl}/api/authentication/me`,
      {
        headers: new HttpHeaders({ Authorization: `Bearer ${token}` }),
      }
    );
  }
}
