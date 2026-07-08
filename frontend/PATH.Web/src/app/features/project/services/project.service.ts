import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ProjectDetail } from '../models/ProjectDetail';
import { Project } from '../models/Project';
import { tap } from 'rxjs';
import { Router } from '@angular/router';
import { CreateProject } from '../models/CreateProject';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private readonly API_URL = `${environment.apiUrl}/projects`;
  private router = inject(Router);
  constructor(private http: HttpClient) {}

  getProjects() {
    return this.http.get<Project[]>(this.API_URL);
  }

  getProjectById(id: string) {
    return this.http.get<ProjectDetail>(`${this.API_URL}/${id}`);
  }

  addMemberToProject(id: string, userId: string) {
    return this.http.post(`${this.API_URL}/${id}/members`, { userId });
  }

  createProject(project: CreateProject) {
    return this.http.post<ProjectDetail>(this.API_URL, project);
  }
}
