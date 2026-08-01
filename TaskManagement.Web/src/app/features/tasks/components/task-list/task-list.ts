import { CdkDragDrop, CdkDropList, CdkDropListGroup } from '@angular/cdk/drag-drop';
import { ChangeDetectionStrategy, Component, computed, effect, input, output, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { Category } from '../../../../shared/models/category.model';
import { Priority, TaskItem, TaskSort, TaskStatus } from '../../../../shared/models/task.model';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { statusLabel } from '../../task-utils';
import { TaskCard } from '../task-card/task-card';

interface StoredFilters { search: string; status: TaskStatus | 'all'; priority: Priority | 'all'; categoryId: string; sort: TaskSort; }

@Component({
  selector: 'app-task-list',
  imports: [CdkDropList, CdkDropListGroup, MatFormFieldModule, MatIconModule, MatInputModule, MatProgressSpinnerModule, MatSelectModule, Pagination, TaskCard],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskList {
  protected readonly TaskStatus = TaskStatus;
  protected readonly Priority = Priority;
  readonly tasks = input<TaskItem[]>([]);
  readonly categories = input<Category[]>([]);
  readonly loading = input(false);
  readonly view = output<TaskItem>();
  readonly edit = output<TaskItem>();
  readonly remove = output<TaskItem>();
  readonly statusChange = output<{ task: TaskItem; status: TaskStatus }>();

  private readonly storageKey = 'task_management_filters';
  private readonly initial = this.readFilters();
  readonly search = signal(this.initial.search);
  readonly status = signal<TaskStatus | 'all'>(this.initial.status);
  readonly priority = signal<Priority | 'all'>(this.initial.priority);
  readonly categoryId = signal(this.initial.categoryId);
  readonly sort = signal<TaskSort>(this.initial.sort);
  readonly page = signal(1);
  readonly pageSize = 6;
  readonly statuses = [TaskStatus.Pending, TaskStatus.InProgress, TaskStatus.Completed, TaskStatus.Cancelled];

  readonly filteredTasks = computed(() => {
    const query = this.search().trim().toLocaleLowerCase('tr');
    const result = this.tasks().filter((task) => {
      const textMatches = !query || `${task.title} ${task.description ?? ''}`.toLocaleLowerCase('tr').includes(query);
      return textMatches
        && (this.status() === 'all' || task.status === this.status())
        && (this.priority() === 'all' || task.priority === this.priority())
        && (!this.categoryId() || task.categoryId === this.categoryId());
    });

    return [...result].sort((a, b) => this.compare(a, b));
  });

  readonly pagedTasks = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filteredTasks().slice(start, start + this.pageSize);
  });

  constructor() {
    effect(() => {
      const filters: StoredFilters = { search: this.search(), status: this.status(), priority: this.priority(), categoryId: this.categoryId(), sort: this.sort() };
      localStorage.setItem(this.storageKey, JSON.stringify(filters));
      this.page.set(1);
    });
  }

  tasksForStatus(status: TaskStatus): TaskItem[] { return this.pagedTasks().filter((task) => task.status === status); }
  statusLabel = statusLabel;
  categoryColor(task: TaskItem): string { return this.categories().find((item) => item.id === task.categoryId)?.color ?? '#94a3b8'; }

  drop(event: CdkDragDrop<TaskItem[]>, status: TaskStatus): void {
    const task = event.item.data as TaskItem;
    if (task.status !== status) this.statusChange.emit({ task, status });
  }

  reset(): void {
    this.search.set(''); this.status.set('all'); this.priority.set('all'); this.categoryId.set(''); this.sort.set('created-desc');
  }

  private compare(a: TaskItem, b: TaskItem): number {
    switch (this.sort()) {
      case 'due-asc': return (a.dueDate ? new Date(a.dueDate).getTime() : Number.MAX_SAFE_INTEGER) - (b.dueDate ? new Date(b.dueDate).getTime() : Number.MAX_SAFE_INTEGER);
      case 'priority-desc': return b.priority - a.priority;
      case 'title-asc': return a.title.localeCompare(b.title, 'tr');
      default: return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    }
  }

  private readFilters(): StoredFilters {
    try {
      const value = JSON.parse(localStorage.getItem(this.storageKey) ?? '{}') as Partial<StoredFilters>;
      return { search: value.search ?? '', status: value.status ?? 'all', priority: value.priority ?? 'all', categoryId: value.categoryId ?? '', sort: value.sort ?? 'created-desc' };
    } catch {
      return { search: '', status: 'all', priority: 'all', categoryId: '', sort: 'created-desc' };
    }
  }
}
