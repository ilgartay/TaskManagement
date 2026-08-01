import { DOCUMENT } from '@angular/common';
import { Inject, Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly storageKey = 'task_management_theme';
  readonly mode = signal<ThemeMode>(this.readInitialTheme());

  constructor(@Inject(DOCUMENT) private readonly document: Document) {
    this.apply(this.mode());
  }

  toggle(): void {
    const nextMode: ThemeMode = this.mode() === 'light' ? 'dark' : 'light';
    this.mode.set(nextMode);
    localStorage.setItem(this.storageKey, nextMode);
    this.apply(nextMode);
  }

  private readInitialTheme(): ThemeMode {
    const stored = localStorage.getItem(this.storageKey);
    if (stored === 'light' || stored === 'dark') {
      return stored;
    }

    return matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private apply(mode: ThemeMode): void {
    this.document.documentElement.classList.toggle('dark-theme', mode === 'dark');
    this.document.documentElement.style.colorScheme = mode;
  }
}
