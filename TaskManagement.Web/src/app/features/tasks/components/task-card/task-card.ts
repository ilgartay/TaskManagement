import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { CdkDrag, CdkDragHandle } from '@angular/cdk/drag-drop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TaskItem, TaskStatus } from '../../../../shared/models/task.model';
import { dueState, priorityLabel, statusLabel, taskProgress } from '../../task-utils';

@Component({
  selector: 'app-task-card',
  imports: [DatePipe, CdkDrag, CdkDragHandle, MatButtonModule, MatCardModule, MatIconModule, MatMenuModule, MatProgressBarModule],
  templateUrl: './task-card.html',
  styleUrl: './task-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskCard {
  protected readonly TaskStatus = TaskStatus;
  readonly task = input.required<TaskItem>();
  readonly categoryColor = input('#94a3b8');
  readonly view = output<TaskItem>();
  readonly edit = output<TaskItem>();
  readonly remove = output<TaskItem>();
  readonly statusChange = output<{ task: TaskItem; status: TaskStatus }>();

  priorityLabel = priorityLabel;
  statusLabel = statusLabel;
  taskProgress = taskProgress;
  dueState = dueState;
}
