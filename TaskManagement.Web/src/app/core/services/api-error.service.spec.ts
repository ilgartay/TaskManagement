import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { ApiErrorService } from './api-error.service';

describe('ApiErrorService', () => {
  let service: ApiErrorService;

  beforeEach(() => {
    service = TestBed.inject(ApiErrorService);
  });

  it('uses the message returned by the API', () => {
    const error = new HttpErrorResponse({ status: 400, error: { message: 'Başlık zorunludur.' } });
    expect(service.getMessage(error)).toBe('Başlık zorunludur.');
  });

  it('shows a connection message when the API cannot be reached', () => {
    const error = new HttpErrorResponse({ status: 0 });
    expect(service.getMessage(error)).toContain('Sunucuya ulaşılamıyor');
  });
});
