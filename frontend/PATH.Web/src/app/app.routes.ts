import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { authGuard } from './core/auth/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { guestGuard } from './core/auth/guest.guard';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProjectComponent } from './features/project/project/project.component';
import { CreateProjectComponent } from './features/project/create-project/create-project.component';
import { roleGuard } from './core/auth/role.guard';
import { LayoutComponent } from './core/layout/layout.component';
import { OrganizationComponent } from './features/organization/organization/organization.component';
import { OrganizationListComponent } from './features/organization/organization-list/organization-list.component';
export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },
      {
        path: 'organizations',
        loadComponent: () =>
          import('./features/organization/organization-list/organization-list.component').then(
            (m) => m.OrganizationListComponent,
          ),
      },
      {
        path: 'organizations/create',
        loadComponent: () =>
          import('./features/organization/create-organization/create-organization.component').then(
            (m) => m.CreateOrganizationComponent,
          ),
      },
      {
        path: 'organizations/:id',
        loadComponent: () =>
          import('./features/organization/organization/organization.component').then(
            (m) => m.OrganizationComponent,
          ),
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/project/projects/projects.component').then(
            (m) => m.ProjectsComponent,
          ),
      },
      {
        path: 'projects/create',
        loadComponent: () =>
          import('./features/project/create-project/create-project.component').then(
            (m) => m.CreateProjectComponent,
          ),
      },
      {
        path: 'projects/:id',
        loadComponent: () =>
          import('./features/project/project/project.component').then(
            (m) => m.ProjectComponent,
          ),
      },
    ],
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(
        (m) => m.RegisterComponent,
      ),
    canActivate: [guestGuard],
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(
        (m) => m.LoginComponent,
      ),
    canActivate: [guestGuard],
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./features/landing-page/landing-page.component').then(
        (m) => m.LandingPageComponent,
      ),
    canActivate: [guestGuard],
  },
  {
    path: 'secret-path',
    loadComponent: () =>
      import('./features/secret-page/secret-page.component').then(
        (m) => m.SecretPageComponent,
      ),
  },
];
