import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TaskComment } from '../../../../shared/models/task.model';

@Component({ selector: 'app-comment-section', imports: [DatePipe, MatButtonModule, MatFormFieldModule, MatIconModule, MatInputModule], templateUrl: './comment-section.html', styleUrl: './comment-section.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class CommentSection {
  readonly comments = input<TaskComment[]>([]);
  readonly saving = input(false);
  readonly add = output<string>();
  readonly value = signal('');

  submit(): void {
    const comment = this.value().trim();
    if (!comment) return;
    this.add.emit(comment);
    this.value.set('');
  }
}
