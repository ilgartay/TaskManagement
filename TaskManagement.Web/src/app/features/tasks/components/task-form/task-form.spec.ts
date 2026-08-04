import { TestBed } from '@angular/core/testing';
import { Priority, TaskStatus, UpdateTaskRequest } from '../../../../shared/models/task.model';
import { TaskForm } from './task-form';

describe('TaskForm', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [TaskForm] });
  });

  it('does not submit a whitespace-only title', () => {
    const fixture = TestBed.createComponent(TaskForm);
    const submitted: UpdateTaskRequest[] = [];
    fixture.componentInstance.submitted.subscribe((value) => submitted.push(value));
    fixture.detectChanges();

    fixture.componentInstance.form.controls.title.setValue('   ');
    fixture.componentInstance.submit();

    expect(submitted).toHaveLength(0);
    expect(fixture.componentInstance.form.controls.title.hasError('pattern')).toBe(true);
  });

  it('normalizes optional fields before submitting', () => {
    const fixture = TestBed.createComponent(TaskForm);
    const submitted: UpdateTaskRequest[] = [];
    fixture.componentInstance.submitted.subscribe((value) => submitted.push(value));
    fixture.detectChanges();

    fixture.componentInstance.form.setValue({
      title: '  Test görevi  ', description: '   ', priority: Priority.High,
      status: TaskStatus.Pending, dueDate: '', categoryId: '',
    });
    fixture.componentInstance.submit();

    expect(submitted[0]).toMatchObject({
      title: 'Test görevi', description: null, categoryId: null, dueDate: null,
    });
  });
});
