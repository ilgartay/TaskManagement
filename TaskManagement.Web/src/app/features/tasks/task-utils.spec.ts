import { describe, expect, it } from 'vitest';
import { Priority, TaskStatus } from '../../shared/models/task.model';
import { priorityLabel, statusLabel, taskProgress } from './task-utils';

describe('task utilities', () => {
  it('returns Turkish priority labels', () => {
    expect(priorityLabel(Priority.Critical)).toBe('Kritik');
  });

  it('returns Turkish status labels', () => {
    expect(statusLabel(TaskStatus.InProgress)).toBe('Devam ediyor');
  });

  it('shows completed tasks at one hundred percent', () => {
    expect(taskProgress(TaskStatus.Completed)).toBe(100);
  });
});
