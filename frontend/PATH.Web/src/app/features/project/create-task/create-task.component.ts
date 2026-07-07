import {
  Component,
  inject,
  input,
  Input,
  output,
  Output,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../services/task.service';
import { CreateTask } from '../models/CreateTask';
import { ProjectMember } from '../models/ProjectMember';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.scss',
})
export class CreateTaskComponent {
  taskModel: CreateTask = {
    title: '',
    description: '',
    status: 'Todo',
    priority: 'Low',
    dueDate: '',
    assignedToId: '',
    projectId: '',
  };

  projectId = input.required<string>();
  members = input<ProjectMember[]>([]);
  closed = output();
  taskCreated = output();
  private taskService = inject(TaskService);

  errorMessage = signal<string[] | null>(null);

  successMessage: string | null = null;

  createTask() {
    this.taskModel.projectId = this.projectId();
    this.taskService.createTask(this.taskModel).subscribe({
      next: () => {
        this.successMessage = 'Task created successfully.';
        setTimeout(() => {
          this.successMessage = null;
          this.taskCreated.emit();
          this.closed.emit();
        }, 500);
      },
      error: (err) => {
        this.errorMessage.set(
          this.normalizeErrors(
            err.error?.message,
            'Task creation failed. Please try again.',
          ),
        );
        console.error('Error creating task:', err);
      },
    });
  }

  private normalizeErrors(
    error: string | string[] | undefined,
    fallback: string,
  ) {
    if (Array.isArray(error)) {
      return error.length > 0 ? error : [fallback];
    }

    if (typeof error === 'string' && error.trim() !== '') {
      return [error];
    }

    return [fallback];
  }

  isFormValid() {
    return (
      this.taskModel.title.trim() !== '' &&
      this.taskModel.description.trim() !== '' &&
      this.taskModel.dueDate !== '' &&
      this.taskModel.assignedToId !== '' &&
      this.projectId() !== ''
    );
  }
}
