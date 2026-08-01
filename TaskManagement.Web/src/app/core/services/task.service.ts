import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, shareReplay, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateTaskRequest,
  TaskAttachment,
  TaskComment,
  TaskFilter,
  TaskItem,
  TaskStats,
  UpdateTaskRequest,
} from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly tasksUrl = `${environment.apiUrl}/tasks`;
  private taskCache$: Observable<TaskItem[]> | null = null;

  constructor(private readonly http: HttpClient) {}

  getAll(forceRefresh = false): Observable<TaskItem[]> {
    if (forceRefresh || !this.taskCache$) {
      this.taskCache$ = this.http.get<TaskItem[]>(this.tasksUrl).pipe(
        shareReplay({ bufferSize: 1, refCount: true }),
      );
    }

    return this.taskCache$;
  }

  getById(id: string): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.tasksUrl}/${id}`);
  }

  getFiltered(filter: TaskFilter): Observable<TaskItem[]> {
    let params = new HttpParams();

    Object.entries(filter).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });

    return this.http.get<TaskItem[]>(`${this.tasksUrl}/filter`, { params });
  }

  getStats(): Observable<TaskStats> {
    return this.http.get<TaskStats>(`${this.tasksUrl}/stats`);
  }

  getOverdue(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(`${this.tasksUrl}/overdue`);
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.tasksUrl, request).pipe(tap(() => this.clearCache()));
  }

  update(id: string, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http
      .put<TaskItem>(`${this.tasksUrl}/${id}`, request)
      .pipe(tap(() => this.clearCache()));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.tasksUrl}/${id}`).pipe(tap(() => this.clearCache()));
  }

  getComments(taskId: string): Observable<TaskComment[]> {
    return this.http.get<TaskComment[]>(`${this.tasksUrl}/${taskId}/comments`);
  }

  addComment(taskId: string, comment: string): Observable<TaskComment> {
    return this.http.post<TaskComment>(`${this.tasksUrl}/${taskId}/comments`, { comment });
  }

  getAttachments(taskId: string): Observable<TaskAttachment[]> {
    return this.http.get<TaskAttachment[]>(`${this.tasksUrl}/${taskId}/attachments`);
  }

  uploadAttachment(taskId: string, file: File): Observable<TaskAttachment> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<TaskAttachment>(`${this.tasksUrl}/${taskId}/attachments`, formData);
  }

  downloadAttachment(attachmentId: string): Observable<Blob> {
    return this.http.get(`${this.tasksUrl}/attachments/${attachmentId}/download`, {
      responseType: 'blob',
    });
  }

  clearCache(): void {
    this.taskCache$ = null;
  }
}
