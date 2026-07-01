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
          err.error?.message || 'Task creation failed. Please try again.',
        );
        console.error('Error creating task:', err);
      },
    });
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

  // validateForm() {
  //   const errors: string[] = [];
  //   if (this.taskModel.title.trim() === '') {
  //     errors.push('Task title is required.');
  //   }
  //   if (this.taskModel.description.trim() === '') {
  //     errors.push('Task description is required.');
  //   }
  //   if (this.taskModel.dueDate === null) {
  //     errors.push('Task due date is required.');
  //   }
  //   if (this.taskModel.assignedToId === '') {
  //     errors.push('Task assignee is required.');
  //   }
  //   if (this.projectId() === '') {
  //     errors.push('Project is required.');
  //   }
  //   this.errorMessage.set(errors.length > 0 ? errors : null);
  //   return errors.length === 0;
  // }
}
