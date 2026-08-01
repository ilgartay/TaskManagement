import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { finalize, forkJoin, filter, switchMap } from 'rxjs';
import { ApiErrorService } from '../../../../core/services/api-error.service';
import { AuthService } from '../../../../core/services/auth.service';
import { CategoryService } from '../../../../core/services/category.service';
import { PerformanceMonitorService } from '../../../../core/services/performance-monitor.service';
import { TaskService } from '../../../../core/services/task.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { Category } from '../../../../shared/models/category.model';
import { TaskItem, TaskStats, TaskStatus, UpdateTaskRequest } from '../../../../shared/models/task.model';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { TaskDetail } from '../../components/task-detail/task-detail';
import { TaskForm } from '../../components/task-form/task-form';
import { TaskList } from '../../components/task-list/task-list';

@Component({
  selector: 'app-dashboard',
  imports: [MatButtonModule, MatCardModule, MatDialogModule, MatIconModule, MatSnackBarModule, MatToolbarModule, TaskForm, TaskList],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard {
  readonly authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  private readonly taskService = inject(TaskService);
  private readonly categoryService = inject(CategoryService);
  private readonly errorService = inject(ApiErrorService);
  private readonly performance = inject(PerformanceMonitorService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);

  readonly tasks = signal<TaskItem[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly stats = signal<TaskStats | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly errorMessage = signal('');
  readonly editingTask = signal<TaskItem | null>(null);

  constructor() { this.loadDashboard(); }

  loadDashboard(forceRefresh = false): void {
    const finishMeasure = this.performance.start('Dashboard yükleme');
    this.loading.set(true);
    this.errorMessage.set('');
    forkJoin({ tasks: this.taskService.getAll(forceRefresh), stats: this.taskService.getStats(), categories: this.categoryService.getAll() })
      .pipe(finalize(() => { this.loading.set(false); finishMeasure(); }), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ tasks, stats, categories }) => { this.tasks.set(tasks); this.stats.set(stats); this.categories.set(categories); },
        error: (error) => this.errorMessage.set(this.errorService.getMessage(error)),
      });
  }

  saveTask(request: UpdateTaskRequest): void {
    const currentTask = this.editingTask();
    this.saving.set(true);
    this.errorMessage.set('');
    const operation = currentTask ? this.taskService.update(currentTask.id, request) : this.taskService.create(request);
    operation.pipe(finalize(() => this.saving.set(false)), takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => { this.editingTask.set(null); this.notify(currentTask ? 'Görev güncellendi.' : 'Görev oluşturuldu.'); this.loadDashboard(true); },
      error: (error) => this.errorMessage.set(this.errorService.getMessage(error)),
    });
  }

  changeStatus(event: { task: TaskItem; status: TaskStatus }): void {
    if (event.task.status === event.status) return;
    this.taskService.update(event.task.id, { title: event.task.title, description: event.task.description, priority: event.task.priority, status: event.status, dueDate: event.task.dueDate, categoryId: event.task.categoryId })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => { this.notify('Görev durumu değiştirildi.'); this.loadDashboard(true); }, error: (error) => this.errorMessage.set(this.errorService.getMessage(error)) });
  }

  editTask(task: TaskItem): void {
    this.editingTask.set(task);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  showDetail(task: TaskItem): void {
    const color = this.categories().find((category) => category.id === task.categoryId)?.color ?? '#94a3b8';
    this.dialog.open(TaskDetail, { width: 'min(720px, 96vw)', maxHeight: '90vh', data: { task, categoryColor: color } });
  }

  deleteTask(task: TaskItem): void {
    this.dialog.open(ConfirmDialog, { width: '390px', data: { title: 'Görev silinsin mi?', message: `“${task.title}” kalıcı olarak silinecek.` } }).afterClosed()
      .pipe(filter(Boolean), switchMap(() => this.taskService.delete(task.id)), takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: () => { this.notify('Görev silindi.'); this.loadDashboard(true); }, error: (error) => this.errorMessage.set(this.errorService.getMessage(error)) });
  }

  private notify(message: string): void { this.snackBar.open(message, 'Kapat', { duration: 2800, horizontalPosition: 'right', verticalPosition: 'top' }); }
}
