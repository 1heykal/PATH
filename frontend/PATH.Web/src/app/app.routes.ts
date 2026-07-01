import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { authGuard } from './auth/auth.guard';
import { LoginComponent } from './login/login.component';
import { guestGuard } from './auth/guest.guard';
import { RegisterComponent } from './register/register.component';
import { ProjectComponent } from './project/project.component';
import { CreateProjectComponent } from './create-project/create-project.component';
import { roleGuard } from './auth/role.guard';
import { LayoutComponent } from './shared/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./projects/projects.component').then(
            (m) => m.ProjectsComponent,
          ),
      },
      {
        path: 'projects/create',
        loadComponent: () =>
          import('./create-project/create-project.component').then(
            (m) => m.CreateProjectComponent,
          ),
      },
      {
        path: 'projects/:id',
        loadComponent: () =>
          import('./project/project.component').then((m) => m.ProjectComponent),
      },
      {
        path: 'admin',
        loadComponent: () =>
          import('./admin/admin.component').then((m) => m.AdminComponent),
        canActivate: [roleGuard(['admin'])],
      },
    ],
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./register/register.component').then((m) => m.RegisterComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./login/login.component').then((m) => m.LoginComponent),
    canActivate: [guestGuard],
  },

  {
    path: 'admin',
    loadComponent: () =>
      import('./admin/admin.component').then((m) => m.AdminComponent),
    canActivate: [roleGuard(['admin'])],
  },
];
