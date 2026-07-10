import { Component, computed, inject, input, signal } from '@angular/core';
import { ProjectService } from '../services/project.service';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateProject } from '../models/CreateProject';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-create-project',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.scss',
})
export class CreateProjectComponent {
  private projectService = inject(ProjectService);

  private router = inject(Router);

  successMessage: string | null = null;
  errorMessage = signal<string[] | null>(null);

  organizationId = input<string>('');

  projectModel: CreateProject = {
    name: '',
    description: '',
    organizationId: '',
  };

  private route = inject(ActivatedRoute);

  createProject() {
    this.projectModel.organizationId =
      this.route.snapshot.queryParamMap.get('orgId') || '';

    this.projectService.createProject(this.projectModel).subscribe({
      next: () => {
        this.successMessage = 'Project created successfully.';
        setTimeout(() => {
          this.successMessage = null;
          this.router.navigate([
            `/organizations/${this.projectModel.organizationId}`,
          ]);
        }, 500); // Clear the success message after 10 seconds and navigate to projects page
      },
      error: (err) => {
        this.errorMessage.set(
          this.normalizeErrors(
            err.error?.message,
            'Project creation failed. Please try again.',
          ),
        );
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
    return this.projectModel.name.trim() !== '';
  }
}
