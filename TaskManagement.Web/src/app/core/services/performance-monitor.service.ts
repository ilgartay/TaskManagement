import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PerformanceMonitorService {
  start(label: string): () => void {
    const startTime = performance.now();

    return () => {
      const duration = Math.round(performance.now() - startTime);
      console.info(`[Performance] ${label}: ${duration} ms`);
    };
  }
}
