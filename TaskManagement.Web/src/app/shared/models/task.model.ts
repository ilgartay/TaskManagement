export enum Priority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4,
  Critical = 5,
}

export enum TaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export interface TaskItem {
  id: string;
  title: string;
  description: string | null;
  priority: Priority;
  status: TaskStatus;
  dueDate: string | null;
  completedAt: string | null;
  userId: string;
  categoryId: string | null;
  categoryName: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
  priority: Priority;
  dueDate?: string | null;
  categoryId?: string | null;
}

export interface UpdateTaskRequest extends CreateTaskRequest {
  status: TaskStatus;
}

export interface TaskFilter {
  priority?: Priority;
  status?: TaskStatus;
  categoryId?: string;
  dueDateFrom?: string;
  dueDateTo?: string;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

export interface TaskStats {
  totalTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  completedTasks: number;
  cancelledTasks: number;
  overdueTasks: number;
}
