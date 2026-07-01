import { Component, inject, signal } from '@angular/core';
import { ProjectService } from '../services/project.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CreateProject } from '../models/CreateProject';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-create-project',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.scss',
})
export class CreateProjectComponent {
  private projectService = inject(ProjectService);

  private userRole = inject(AuthService).currentUser()?.role;
  userAllowedToCreateProject =
    this.userRole === 'admin' || this.userRole === 'manager';
  private router = inject(Router);

  successMessage: string | null = null;
  errorMessage = signal<string[] | null>(null);

  projectModel: CreateProject = {
    name: '',
    description: '',
  };

  createProject() {
    if (!this.userAllowedToCreateProject) {
      console.error('User is not allowed to create a project.');
      return;
    }

    // if (!this.validateForm()) return;

    this.projectService.createProject(this.projectModel).subscribe({
      next: () => {
        this.successMessage = 'Project created successfully.';
        setTimeout(() => {
          this.successMessage = null;
          this.router.navigate(['/projects']);
        }, 500); // Clear the success message after 10 seconds and navigate to projects page
      },
      error: (err) => {
        this.errorMessage.set(
          err.error?.message || 'Project creation failed. Please try again.',
        );
        console.error('Error creating project:', err);
      },
    });
  }

  isFormValid() {
    return this.projectModel.name.trim() !== '';
  }

  // validateForm() {
  //   const errors: string[] = [];
  //   if (this.projectModel.name.trim() === '') {
  //     errors.push('Project name is required.');
  //   }

  //   if (errors.length > 0) this.errorMessage.set(errors);

  //   return errors.length === 0;
  // }
}
