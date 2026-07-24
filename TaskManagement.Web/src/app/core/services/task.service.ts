import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateTaskRequest,
  TaskFilter,
  TaskItem,
  TaskStats,
  UpdateTaskRequest,
} from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly tasksUrl = `${environment.apiUrl}/tasks`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.tasksUrl);
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
    return this.http.post<TaskItem>(this.tasksUrl, request);
  }

  update(id: string, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.tasksUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.tasksUrl}/${id}`);
  }
}
