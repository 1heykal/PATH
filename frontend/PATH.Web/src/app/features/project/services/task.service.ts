import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Task } from '../models/Task';
import { Status } from '../models/Types';
import { CreateTask } from '../models/CreateTask';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private API_URL = `${environment.apiUrl}/tasks`;
  private http = inject(HttpClient);
  constructor() {}

  createTask(task: CreateTask) {
    return this.http.post<Task>(this.API_URL, task);
  }

  updateTaskStatus(taskId: string, newStatus: Status) {
    return this.http.patch<Task>(`${this.API_URL}/${taskId}/status`, {
      newStatus,
    });
  }

  assignTaskToUser(taskId: string, userId: string) {
    return this.http.patch<Task>(`${this.API_URL}/${taskId}/assign`, {
      assignedToId: userId,
    });
  }

  deleteTask(taskId: string) {
    return this.http.delete(`${this.API_URL}/${taskId}`);
  }
}
