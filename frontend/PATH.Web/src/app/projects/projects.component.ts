import { Component, inject, OnInit, signal, Pipe } from '@angular/core';
import { ProjectService } from '../services/project.service';
import { Project } from '../models/Project';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { SpinnerComponent } from '../shared/spinner/spinner.component';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [RouterLink, DatePipe, SpinnerComponent],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss',
})
export class ProjectsComponent {
  projects = signal<Project[]>([]);
  isLoading = signal<boolean>(true);
  private projectService = inject(ProjectService);
  private router = inject(Router);
  constructor() {
    this.projectService.getProjects().subscribe({
      next: (data) => {
        this.projects.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  createProject() {
    this.router.navigate(['/projects/create']);
  }
}
