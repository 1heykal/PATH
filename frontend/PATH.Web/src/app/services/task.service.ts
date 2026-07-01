import { inject, Injectable } from '@angular/core';
import { environment } from '../../environment';
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
    return this.http.post<Task>(this.API_URL, task).pipe(
      tap({
        next: (response) => {
          console.log('Task created successfully:', response);
        },
        error: (err) => {
          console.error('Error creating task:', err);
        },
      }),
    );
  }

  updateTaskStatus(taskId: string, newStatus: Status) {
    return this.http
      .patch<Task>(`${this.API_URL}/${taskId}/status`, {
        newStatus,
      })
      .pipe(
        tap({
          next: (response) => {
            console.log('Task status updated successfully:', response);
          },
          error: (err) => {
            console.error('Error updating task status:', err);
          },
        }),
      );
  }

  assignTaskToUser(taskId: string, userId: string) {
    return this.http
      .patch<Task>(`${this.API_URL}/${taskId}/assign`, {
        assignedToId: userId,
      })
      .pipe(
        tap({
          next: (response) => {
            console.log('Task assigned successfully:', response);
          },
          error: (err) => {
            console.error('Error assigning task:', err);
          },
        }),
      );
  }
}
