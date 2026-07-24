import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from '../services/token.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const token = tokenService.getToken();
  const isApiRequest = request.url.startsWith(environment.apiUrl);

  const authorizedRequest =
    token && isApiRequest
      ? request.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        })
      : request;

  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthRequest =
        request.url.includes('/auth/login') || request.url.includes('/auth/register');

      if (error.status === 401 && !isAuthRequest) {
        tokenService.clearSession();
        void router.navigate(['/login'], { queryParams: { sessionExpired: true } });
      }

      return throwError(() => error);
    }),
  );
};
