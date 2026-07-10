import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ProjectService } from '../services/project.service';
import { ActivatedRoute } from '@angular/router';
import { ProjectDetail } from '../models/ProjectDetail';
import { CommonModule, DatePipe } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';
import { CreateTaskComponent } from '../create-task/create-task.component';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../services/task.service';
import { Status } from '../models/Types';
import { finalize } from 'rxjs';
import { SpinnerComponent } from '../../../shared/spinner/spinner.component';
import { OrgMember } from '../../organization/models/OrgMember';
import { OrganizationService } from '../../organization/services/organization.service';

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

  private userRole = inject(AuthService).userCurrentOrgRole;
  public currentUserId = inject(AuthService).currentUser()?.id;

  userAllowedToCreateTask = computed(
    () => this.userRole() === 'Admin' || this.userRole() === 'Manager',
  );

  userAllowedToUpdateStatus = computed(
    () => this.userRole() === 'Admin' || this.userRole() === 'Manager',
  );
  canDeleteTask = computed(() => this.userRole() === 'Admin');
  canAddProjectMembers = computed(() => this.userRole() === 'Admin');

  showCreateTask = signal(false);
  deleteConfirmTaskId = signal<string | null>(null);

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
        this.userRole.set(data.currentUserRole);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  addedMemberId: string | null = null;

  addMemberToProject() {
    if (this.addedMemberId) {
      this.projectService
        .addMemberToProject(this.projectId, this.addedMemberId)
        .subscribe({
          next: () => {
            this.orgMembersLoaded = false;
            this.loadProject();
            this.addedMemberId = null;
          },
        });
    }
  }

  updateTaskStatus(taskId: string, newStatus: Status) {
    if (!this.project()) {
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

  isUserAllowedToAssignTask = computed(
    () => this.userRole() === 'Admin' || this.userRole() === 'Manager',
  );

  assignTaskToUser(taskId: string, userId: string) {
    if (!this.project()) {
      return;
    }

    if (this.userAllowedToCreateTask()) {
      this.taskService
        .assignTaskToUser(taskId, userId)
        .pipe(
          finalize(() => {
            this.loadProject();
          }),
        )
        .subscribe();
    }
  }

  openDeleteTaskConfirm(taskId: string) {
    if (!this.canDeleteTask()) {
      return;
    }

    this.deleteConfirmTaskId.set(taskId);
  }

  closeDeleteTaskConfirm() {
    this.deleteConfirmTaskId.set(null);
  }

  confirmDeleteTask() {
    const taskId = this.deleteConfirmTaskId();

    if (!taskId || !this.project()) {
      return;
    }

    this.taskService
      .deleteTask(taskId)
      .pipe(
        finalize(() => {
          this.closeDeleteTaskConfirm();
          this.loadProject();
        }),
      )
      .subscribe();
  }

  organizationMembers: OrgMember[] = [];
  orgMembersLoaded = false;
  private orgService = inject(OrganizationService);

  loadOrgMembers() {
    if (!this.orgMembersLoaded) {
      this.orgService
        .getOrgMembers(this.project()!.organizationId)
        .subscribe((response) => {
          const projectMemberIds = this.project()!.members.map(
            (m) => m.memberId,
          );

          this.organizationMembers = response.filter(
            (member) => !projectMemberIds.includes(member.userId),
          );

          this.orgMembersLoaded = true;
        });
    }
  }
  ngOnInit() {
    this.loadProject();
  }
}
