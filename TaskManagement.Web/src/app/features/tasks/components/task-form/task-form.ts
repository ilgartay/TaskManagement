import { ChangeDetectionStrategy, Component, effect, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Category } from '../../../../shared/models/category.model';
import { Priority, TaskItem, TaskStatus, UpdateTaskRequest } from '../../../../shared/models/task.model';

@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule, MatInputModule, MatSelectModule, MatProgressBarModule],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskForm {
  protected readonly Priority = Priority;
  protected readonly TaskStatus = TaskStatus;
  readonly task = input<TaskItem | null>(null);
  readonly categories = input<Category[]>([]);
  readonly saving = input(false);
  readonly submitted = output<UpdateTaskRequest>();
  readonly cancelled = output<void>();

  readonly form;

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.nonNullable.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      priority: [Priority.Normal, Validators.required],
      status: [TaskStatus.Pending, Validators.required],
      dueDate: [''],
      categoryId: [''],
    });

    effect(() => {
      const task = this.task();
      this.form.reset({
        title: task?.title ?? '',
        description: task?.description ?? '',
        priority: task?.priority ?? Priority.Normal,
        status: task?.status ?? TaskStatus.Pending,
        dueDate: task?.dueDate?.slice(0, 10) ?? '',
        categoryId: task?.categoryId ?? '',
      });
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitted.emit({
      title: value.title.trim(),
      description: value.description.trim() || null,
      priority: value.priority,
      status: value.status,
      dueDate: value.dueDate ? new Date(`${value.dueDate}T23:59:59`).toISOString() : null,
      categoryId: value.categoryId || null,
    });
  }
}
