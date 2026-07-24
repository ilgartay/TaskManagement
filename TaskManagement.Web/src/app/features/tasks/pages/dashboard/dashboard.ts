import { DatePipe } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { finalize, forkJoin } from 'rxjs';
import { ApiErrorService } from '../../../../core/services/api-error.service';
import { AuthService } from '../../../../core/services/auth.service';
import { CategoryService } from '../../../../core/services/category.service';
import { TaskService } from '../../../../core/services/task.service';
import { Category } from '../../../../shared/models/category.model';
import {
  CreateTaskRequest,
  Priority,
  TaskItem,
  TaskStats,
  TaskStatus,
} from '../../../../shared/models/task.model';

@Component({
  selector: 'app-dashboard',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatToolbarModule,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  protected readonly Priority = Priority;
  protected readonly TaskStatus = TaskStatus;

  readonly tasks = signal<TaskItem[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly stats = signal<TaskStats | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly errorMessage = signal('');

  readonly taskForm;

  constructor(
    formBuilder: FormBuilder,
    readonly authService: AuthService,
    private readonly taskService: TaskService,
    private readonly categoryService: CategoryService,
    private readonly apiErrorService: ApiErrorService,
  ) {
    this.taskForm = formBuilder.nonNullable.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      priority: [Priority.Normal, Validators.required],
      dueDate: [''],
      categoryId: [''],
    });

    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    forkJoin({
      tasks: this.taskService.getAll(),
      stats: this.taskService.getStats(),
      categories: this.categoryService.getAll(),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ tasks, stats, categories }) => {
          this.tasks.set(tasks);
          this.stats.set(stats);
          this.categories.set(categories);
        },
        error: (error: unknown) => this.errorMessage.set(this.apiErrorService.getMessage(error)),
      });
  }

  createTask(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const formValue = this.taskForm.getRawValue();
    const request: CreateTaskRequest = {
      title: formValue.title.trim(),
      priority: formValue.priority,
      dueDate: formValue.dueDate ? new Date(`${formValue.dueDate}T23:59:59`).toISOString() : null,
      categoryId: formValue.categoryId || null,
    };

    this.saving.set(true);
    this.errorMessage.set('');

    this.taskService
      .create(request)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.taskForm.reset({
            title: '',
            priority: Priority.Normal,
            dueDate: '',
            categoryId: '',
          });
          this.loadDashboard();
        },
        error: (error: unknown) => this.errorMessage.set(this.apiErrorService.getMessage(error)),
      });
  }

  completeTask(task: TaskItem): void {
    this.taskService
      .update(task.id, {
        title: task.title,
        description: task.description,
        priority: task.priority,
        status: TaskStatus.Completed,
        dueDate: task.dueDate,
        categoryId: task.categoryId,
      })
      .subscribe({
        next: () => this.loadDashboard(),
        error: (error: unknown) => this.errorMessage.set(this.apiErrorService.getMessage(error)),
      });
  }

  deleteTask(task: TaskItem): void {
    const confirmed = window.confirm(`"${task.title}" görevini silmek istiyor musunuz?`);

    if (!confirmed) {
      return;
    }

    this.taskService.delete(task.id).subscribe({
      next: () => this.loadDashboard(),
      error: (error: unknown) => this.errorMessage.set(this.apiErrorService.getMessage(error)),
    });
  }

  priorityLabel(priority: Priority): string {
    switch (priority) {
      case Priority.Low:
        return 'Düşük';
      case Priority.Normal:
        return 'Normal';
      case Priority.High:
        return 'Yüksek';
      case Priority.Urgent:
        return 'Acil';
      case Priority.Critical:
        return 'Kritik';
      default:
        return 'Bilinmiyor';
    }
  }

  statusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Pending:
        return 'Bekliyor';
      case TaskStatus.InProgress:
        return 'Devam ediyor';
      case TaskStatus.Completed:
        return 'Tamamlandı';
      case TaskStatus.Cancelled:
        return 'İptal edildi';
      default:
        return 'Bilinmiyor';
    }
  }
}
