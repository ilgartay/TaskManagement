import { Injectable } from '@angular/core';
import { User } from '../../shared/models/user.model';

interface JwtPayload {
  exp?: number;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly tokenKey = 'task_management_token';
  private readonly userKey = 'task_management_user';

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  saveSession(token: string, user: User): void {
    localStorage.setItem(this.tokenKey, token);
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  getUser(): User | null {
    const storedUser = localStorage.getItem(this.userKey);

    if (!storedUser) {
      return null;
    }

    try {
      return JSON.parse(storedUser) as User;
    } catch {
      this.clearSession();
      return null;
    }
  }

  clearSession(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  getTokenExpiration(): Date | null {
    const token = this.getToken();

    if (!token) {
      return null;
    }

    try {
      const encodedPayload = token.split('.')[1];
      const normalizedPayload = encodedPayload.replace(/-/g, '+').replace(/_/g, '/');
      const paddedPayload = normalizedPayload.padEnd(
        Math.ceil(normalizedPayload.length / 4) * 4,
        '=',
      );
      const payload = JSON.parse(atob(paddedPayload)) as JwtPayload;

      return payload.exp ? new Date(payload.exp * 1000) : null;
    } catch {
      return null;
    }
  }

  isTokenExpired(): boolean {
    const expiration = this.getTokenExpiration();
    return !expiration || expiration.getTime() <= Date.now();
  }
}
