import { computed, effect, Injectable, signal } from '@angular/core';
import { User } from '../models/User';
import { HttpClient } from '@angular/common/http';
import { RegisterUserModel } from '../models/RegisterUserModel';
import { LoginResponse } from '../models/LoginResponse';
import { catchError, of, tap } from 'rxjs';
import { AccessModel } from '../models/AccessModel';
import { Router } from '@angular/router';
import { environment } from '../../environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly API_URL = `${environment.apiUrl}/auth`;

  currentUser = signal<User | null>(null);
  isLoggedIn = computed(() => this.currentUser() !== null);

  private accessToken: string | null = null;
  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  login(accessModel: AccessModel) {
    return this.http
      .post<LoginResponse>(`${this.API_URL}/Login`, accessModel, {
        withCredentials: true,
      })
      .pipe(
        tap({
          next: (response) => {
            this.accessToken = response.accessToken;
            this.currentUser.set(response.user);
            console.log('Login successful:', response);
            this.router.navigate(['/dashboard']);
          },
          error: (err) => {
            console.error(err);
          },
        }),
      );
  }

  logout() {
    return this.http
      .post(`${this.API_URL}/logout`, {}, { withCredentials: true })
      .pipe(
        tap({
          next: (response) => {
            this.accessToken = null;
            this.currentUser.set(null);
            this.router.navigate(['/login']);
          },
          error: (err) => {
            console.error(err);
          },
        }),
      );
  }

  register(registerUserModel: RegisterUserModel) {
    return this.http.post(`${this.API_URL}/Register`, registerUserModel).pipe(
      tap({
        next: (response) => {
          console.log(response);
        },
        error: (err) => {
          console.error(err);
        },
      }),
    );
  }

  refresh() {
    return this.http
      .post<LoginResponse>(
        `${this.API_URL}/Refresh`,
        {},
        { withCredentials: true },
      )
      .pipe(
        tap({
          next: (response) => {
            this.accessToken = response.accessToken;
            this.currentUser.set(response.user);
          },
          error: (err) => {
            console.error(err);
            this.currentUser.set(null);
            // this.router.navigate(['login']);
          },
        }),
        catchError(() => of(null)),
      );
  }

  getAccessToken() {
    return this.accessToken;
  }

  test() {
    return this.http.get(`${this.API_URL}/test`);
  }

  checkUserRole = effect(() => {
    const user = this.currentUser();
    if (user?.role !== 'admin' && this.router.url.includes('/admin')) {
      this.router.navigate(['/dashboard']);
    }
  });
}
