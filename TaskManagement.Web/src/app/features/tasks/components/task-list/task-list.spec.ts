import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { Priority, TaskItem, TaskStatus } from '../../../../shared/models/task.model';
import { TaskList } from './task-list';

describe('TaskList', () => {
  const values = new Map<string, string>();
  const storage = {
    clear: () => values.clear(),
    getItem: (key: string) => values.get(key) ?? null,
    removeItem: (key: string) => values.delete(key),
    setItem: (key: string, value: string) => values.set(key, value),
  };

  beforeAll(() => vi.stubGlobal('localStorage', storage));
  afterAll(() => vi.unstubAllGlobals());

  beforeEach(() => {
    storage.clear();
    TestBed.configureTestingModule({ imports: [TaskList] });
  });

  it('filters and searches tasks', () => {
    const fixture = TestBed.createComponent(TaskList);
    fixture.componentRef.setInput('tasks', [createTask(1, 'Haftalık rapor', Priority.High), createTask(2, 'Toplantı', Priority.Low)]);
    fixture.detectChanges();

    fixture.componentInstance.search.set('rapor');
    fixture.componentInstance.priority.set(Priority.High);
    TestBed.flushEffects();

    expect(fixture.componentInstance.filteredTasks().map((item) => item.title)).toEqual(['Haftalık rapor']);
    expect(localStorage.getItem('task_management_filters')).toContain('rapor');
  });

  it('sorts tasks and paginates six items at a time', () => {
    const fixture = TestBed.createComponent(TaskList);
    const tasks = Array.from({ length: 7 }, (_, index) => createTask(index + 1, `Task ${index + 1}`, index === 0 ? Priority.Critical : Priority.Normal));
    fixture.componentRef.setInput('tasks', tasks);
    fixture.detectChanges();

    fixture.componentInstance.sort.set('priority-desc');
    expect(fixture.componentInstance.filteredTasks()[0].priority).toBe(Priority.Critical);
    expect(fixture.componentInstance.pagedTasks()).toHaveLength(6);

    fixture.componentInstance.page.set(2);
    expect(fixture.componentInstance.pagedTasks()).toHaveLength(1);
  });
});

function createTask(index: number, title: string, priority: Priority): TaskItem {
  return {
    id: `task-${index}`, title, description: null, priority, status: TaskStatus.Pending,
    dueDate: null, completedAt: null, userId: 'user-1', categoryId: null, categoryName: null,
    createdAt: `2026-08-${String(index).padStart(2, '0')}T10:00:00Z`,
    updatedAt: `2026-08-${String(index).padStart(2, '0')}T10:00:00Z`,
  };
}
