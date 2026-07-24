import { HttpClient } from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../../shared/models/user.model';
import { TokenService } from './token.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authUrl = `${environment.apiUrl}/auth`;
  private readonly currentUserSignal = signal<User | null>(null);
  private logoutTimer: ReturnType<typeof setTimeout> | null = null;

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAuthenticated = computed(
    () => this.currentUserSignal() !== null && !this.tokenService.isTokenExpired(),
  );

  constructor(
    private readonly http: HttpClient,
    private readonly tokenService: TokenService,
    private readonly router: Router,
  ) {
    this.currentUserSignal.set(this.tokenService.getUser());
    this.restoreSession();
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, request)
      .pipe(tap((response) => this.startSession(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/register`, request)
      .pipe(tap((response) => this.startSession(response)));
  }

  logout(redirect = true): void {
    this.clearLogoutTimer();
    this.tokenService.clearSession();
    this.currentUserSignal.set(null);

    if (redirect) {
      void this.router.navigate(['/login']);
    }
  }

  private startSession(response: AuthResponse): void {
    this.tokenService.saveSession(response.token, response.user);
    this.currentUserSignal.set(response.user);
    this.scheduleAutoLogout();
  }

  private restoreSession(): void {
    if (!this.tokenService.getToken() || this.tokenService.isTokenExpired()) {
      this.logout(false);
      return;
    }

    this.scheduleAutoLogout();
  }

  private scheduleAutoLogout(): void {
    this.clearLogoutTimer();

    const expiration = this.tokenService.getTokenExpiration();

    if (!expiration) {
      this.logout(false);
      return;
    }

    const remainingTime = expiration.getTime() - Date.now();
    this.logoutTimer = setTimeout(() => this.logout(), remainingTime);
  }

  private clearLogoutTimer(): void {
    if (this.logoutTimer) {
      clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    }
  }
}
