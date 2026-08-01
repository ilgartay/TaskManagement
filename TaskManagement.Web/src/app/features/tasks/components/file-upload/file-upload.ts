import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-file-upload',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './file-upload.html',
  styleUrl: './file-upload.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileUpload {
  readonly uploading = input(false);
  readonly selected = output<File>();
  readonly error = signal('');

  choose(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;
    if (file.size > 10 * 1024 * 1024) { this.error.set('Dosya en fazla 10 MB olabilir.'); return; }
    this.error.set('');
    this.selected.emit(file);
  }
}
