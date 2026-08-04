import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { vi } from 'vitest';
import { environment } from '../../../environments/environment';
import { TokenService } from '../services/token.service';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  const tokenService = {
    getToken: vi.fn(),
    clearSession: vi.fn(),
  };
  const router = { navigate: vi.fn().mockResolvedValue(true) };
  let client: HttpClient;
  let http: HttpTestingController;

  beforeEach(() => {
    vi.clearAllMocks();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: TokenService, useValue: tokenService },
        { provide: Router, useValue: router },
      ],
    });
    client = TestBed.inject(HttpClient);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('adds the bearer token only to API requests', () => {
    tokenService.getToken.mockReturnValue('test-token');
    client.get(`${environment.apiUrl}/tasks`).subscribe();

    const request = http.expectOne(`${environment.apiUrl}/tasks`);
    expect(request.request.headers.get('Authorization')).toBe('Bearer test-token');
    request.flush([]);
  });

  it('clears the session after an unauthorized protected request', () => {
    tokenService.getToken.mockReturnValue('expired-token');
    client.get(`${environment.apiUrl}/tasks`).subscribe({ error: () => undefined });

    http.expectOne(`${environment.apiUrl}/tasks`).flush({}, { status: 401, statusText: 'Unauthorized' });

    expect(tokenService.clearSession).toHaveBeenCalledOnce();
    expect(router.navigate).toHaveBeenCalledWith(['/login'], { queryParams: { sessionExpired: true } });
  });
});
