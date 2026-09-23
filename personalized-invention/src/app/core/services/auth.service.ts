import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponse, CurrentUser, LoginRequest, RegisterRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(
    this.loadUserFromStorage()
  );

  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  get currentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  get isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null && !this.isTokenExpired();
  }

  get isAdmin(): boolean {
    return this.currentUserSubject.value?.isAdmin === true;
  }

  get userId(): number {
    return this.currentUserSubject.value?.userId ?? 0;
  }

  get token(): string | null {
    return localStorage.getItem('pi_token');
  }

  register(dto: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, dto).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  logout(): void {
    localStorage.removeItem('pi_token');
    localStorage.removeItem('pi_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  private handleAuthSuccess(response: AuthResponse): void {
    localStorage.setItem('pi_token', response.token);

    const user: CurrentUser = {
      userId:   response.userId,
      fullName: response.fullName,
      email:    response.email,
      isAdmin:  response.isAdmin    // ← new
    };
    localStorage.setItem('pi_user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private loadUserFromStorage(): CurrentUser | null {
    try {
      const stored = localStorage.getItem('pi_user');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  private isTokenExpired(): boolean {
    const token = this.token;
    if (!token) return true;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }
}