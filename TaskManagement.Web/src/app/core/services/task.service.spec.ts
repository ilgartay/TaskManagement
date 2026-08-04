import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { Priority, TaskItem, TaskStatus } from '../../shared/models/task.model';
import { TaskService } from './task.service';

describe('TaskService', () => {
  let service: TaskService;
  let http: HttpTestingController;

  const task: TaskItem = {
    id: 'task-1', title: 'Test task', description: null, priority: Priority.Normal,
    status: TaskStatus.Pending, dueDate: null, completedAt: null, userId: 'user-1',
    categoryId: null, categoryName: null, createdAt: '2026-08-04T10:00:00Z',
    updatedAt: '2026-08-04T10:00:00Z',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(TaskService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('caches the task list until a mutation happens', () => {
    service.getAll().subscribe((items) => expect(items).toEqual([task]));
    service.getAll().subscribe((items) => expect(items).toEqual([task]));
    http.expectOne(`${environment.apiUrl}/tasks`).flush([task]);

    service.create({ title: 'New task', priority: Priority.High }).subscribe();
    http.expectOne(`${environment.apiUrl}/tasks`).flush(task);

    service.getAll().subscribe();
    http.expectOne(`${environment.apiUrl}/tasks`).flush([task]);
  });

  it('sends filter values as query parameters', () => {
    service.getFiltered({ searchTerm: 'rapor', page: 2, pageSize: 5 }).subscribe();

    const request = http.expectOne((item) => item.url === `${environment.apiUrl}/tasks/filter`);
    expect(request.request.params.get('searchTerm')).toBe('rapor');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('pageSize')).toBe('5');
    request.flush([]);
  });

  it('uploads the selected file as multipart form data', () => {
    const file = new File(['content'], 'notes.txt', { type: 'text/plain' });
    service.uploadAttachment('task-1', file).subscribe();

    const request = http.expectOne(`${environment.apiUrl}/tasks/task-1/attachments`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toBeInstanceOf(FormData);
    expect((request.request.body as FormData).get('file')).toBe(file);
    request.flush({});
  });
});
