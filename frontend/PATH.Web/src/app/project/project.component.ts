import { Component, inject, OnInit, signal } from '@angular/core';
import { ProjectService } from '../services/project.service';
import { ActivatedRoute } from '@angular/router';
import { ProjectDetail } from '../models/ProjectDetail';
import { CommonModule, DatePipe } from '@angular/common';
import { AuthService } from '../auth/auth.service';
import { CreateTaskComponent } from '../create-task/create-task.component';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../services/task.service';
import { Status } from '../models/Types';
import { finalize } from 'rxjs';
import { SpinnerComponent } from '../shared/spinner/spinner.component';

@Component({
  selector: 'app-project',
  standalone: true,
  imports: [
    DatePipe,
    CreateTaskComponent,
    CommonModule,
    FormsModule,
    SpinnerComponent,
  ],
  templateUrl: './project.component.html',
  styleUrl: './project.component.scss',
})
export class ProjectComponent implements OnInit {
  project = signal<ProjectDetail | null>(null);

  isLoading = signal<boolean>(true);

  private route = inject(ActivatedRoute);
  private projectService = inject(ProjectService);
  private taskService = inject(TaskService);

  private userRole = inject(AuthService).currentUser()?.role;
  userAllowedToCreateTask =
    this.userRole === 'admin' || this.userRole === 'manager';

  showCreateTask = signal(false);

  onTaskCreated() {
    this.loadProject();
  }

  projectId: string = '';
  constructor() {
    this.projectId = this.route.snapshot.paramMap.get('id') || '';
  }

  loadProject() {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (data) => {
        this.project.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  updateTaskStatus(taskId: string, newStatus: Status) {
    if (!this.project()) {
      console.error('Project data is not loaded yet.');
      return;
    }

    this.taskService
      .updateTaskStatus(taskId, newStatus)
      .pipe(
        finalize(() => {
          this.loadProject();
        }),
      )
      .subscribe();
  }

  assignTaskToUser(taskId: string, userId: string) {
    if (!this.project()) {
      console.error('Project data is not loaded yet.');
      return;
    }

    this.taskService
      .assignTaskToUser(taskId, userId)
      .pipe(
        finalize(() => {
          this.loadProject();
        }),
      )
      .subscribe();
  }

  ngOnInit() {
    this.loadProject();
  }
}
