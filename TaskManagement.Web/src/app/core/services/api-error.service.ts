import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiError } from '../../shared/models/api-response.model';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  getMessage(error: unknown): string {
    if (!(error instanceof HttpErrorResponse)) {
      return 'Beklenmeyen bir hata oluştu.';
    }

    const apiError = error.error as Partial<ApiError> | null;

    if (apiError?.message) {
      return apiError.message;
    }

    if (error.status === 0) {
      return 'Sunucuya ulaşılamıyor. API projesinin çalıştığını kontrol edin.';
    }

    if (error.status === 401) {
      return 'Oturum bilgileriniz geçersiz.';
    }

    return 'İşlem tamamlanamadı. Lütfen tekrar deneyin.';
  }
}
