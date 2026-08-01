import { Priority, TaskItem, TaskStatus } from '../../shared/models/task.model';

export function priorityLabel(priority: Priority): string {
  return ['Bilinmiyor', 'Düşük', 'Normal', 'Yüksek', 'Acil', 'Kritik'][priority] ?? 'Bilinmiyor';
}

export function statusLabel(status: TaskStatus): string {
  return ['Bekliyor', 'Devam ediyor', 'Tamamlandı', 'İptal edildi'][status] ?? 'Bilinmiyor';
}

export function taskProgress(status: TaskStatus): number {
  if (status === TaskStatus.Completed) return 100;
  if (status === TaskStatus.InProgress) return 55;
  if (status === TaskStatus.Cancelled) return 0;
  return 15;
}

export function dueState(task: TaskItem): 'none' | 'soon' | 'overdue' {
  if (!task.dueDate || task.status === TaskStatus.Completed || task.status === TaskStatus.Cancelled) {
    return 'none';
  }

  const difference = new Date(task.dueDate).getTime() - Date.now();
  if (difference < 0) return 'overdue';
  return difference < 86_400_000 * 2 ? 'soon' : 'none';
}
