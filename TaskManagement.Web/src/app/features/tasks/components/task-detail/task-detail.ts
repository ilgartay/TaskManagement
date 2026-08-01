import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MAT_DIALOG_DATA, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { finalize, forkJoin } from 'rxjs';
import { ApiErrorService } from '../../../../core/services/api-error.service';
import { TaskService } from '../../../../core/services/task.service';
import { TaskAttachment, TaskComment, TaskItem } from '../../../../shared/models/task.model';
import { dueState, priorityLabel, statusLabel, taskProgress } from '../../task-utils';
import { CommentSection } from '../comment-section/comment-section';
import { FileUpload } from '../file-upload/file-upload';

export interface TaskDetailData { task: TaskItem; categoryColor: string; }

@Component({ selector: 'app-task-detail', imports: [DatePipe, DecimalPipe, MatDialogTitle, MatDialogContent, MatButtonModule, MatDividerModule, MatIconModule, MatProgressBarModule, CommentSection, FileUpload], templateUrl: './task-detail.html', styleUrl: './task-detail.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class TaskDetail {
  readonly data = inject<TaskDetailData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<TaskDetail>);
  private readonly taskService = inject(TaskService);
  private readonly errorService = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  readonly comments = signal<TaskComment[]>([]);
  readonly attachments = signal<TaskAttachment[]>([]);
  readonly loading = signal(true);
  readonly savingComment = signal(false);
  readonly uploading = signal(false);
  readonly error = signal('');
  priorityLabel = priorityLabel; statusLabel = statusLabel; taskProgress = taskProgress; dueState = dueState;

  constructor() { this.loadRelated(); }
  close(): void { this.dialogRef.close(); }

  addComment(comment: string): void {
    this.savingComment.set(true);
    this.taskService.addComment(this.data.task.id, comment).pipe(finalize(() => this.savingComment.set(false)), takeUntilDestroyed(this.destroyRef)).subscribe({ next: (item) => this.comments.update((items) => [...items, item]), error: (error) => this.error.set(this.errorService.getMessage(error)) });
  }

  upload(file: File): void {
    this.uploading.set(true);
    this.taskService.uploadAttachment(this.data.task.id, file).pipe(finalize(() => this.uploading.set(false)), takeUntilDestroyed(this.destroyRef)).subscribe({ next: (item) => this.attachments.update((items) => [...items, item]), error: (error) => this.error.set(this.errorService.getMessage(error)) });
  }

  download(item: TaskAttachment): void {
    this.taskService.downloadAttachment(item.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({ next: (blob) => { const url = URL.createObjectURL(blob); const anchor = document.createElement('a'); anchor.href = url; anchor.download = item.fileName; anchor.click(); URL.revokeObjectURL(url); }, error: (error) => this.error.set(this.errorService.getMessage(error)) });
  }

  private loadRelated(): void {
    forkJoin({ comments: this.taskService.getComments(this.data.task.id), attachments: this.taskService.getAttachments(this.data.task.id) }).pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef)).subscribe({ next: ({ comments, attachments }) => { this.comments.set(comments); this.attachments.set(attachments); }, error: (error) => this.error.set(this.errorService.getMessage(error)) });
  }
}
