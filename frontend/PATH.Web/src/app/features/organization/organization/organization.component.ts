import { Component, computed, inject } from '@angular/core';
import { OrganizationService } from '../services/organization.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Organization } from '../models/Organization';
import { SpinnerComponent } from '../../../shared/spinner/spinner.component';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ProjectsComponent } from '../../project/projects/projects.component';
import { AuthService } from '../../../core/auth/auth.service';
import { UserService } from '../../auth/services/user.service';
import { tap, finalize } from 'rxjs';

@Component({
  selector: 'app-organization',
  standalone: true,
  imports: [
    SpinnerComponent,
    FormsModule,
    RouterLink,
    DatePipe,
    ProjectsComponent,
  ],
  templateUrl: './organization.component.html',
  styleUrl: './organization.component.scss',
})
export class OrganizationComponent {
  // TODO: Implement the organization component
  private organizationService = inject(OrganizationService);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);

  organization: Organization | null = null;
  organizationId: string = '';
  isLoading: boolean = true;

  memberEmail = '';
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor() {
    this.organizationId = this.route.snapshot.paramMap.get('id') ?? '';
    this.getOrganization(this.organizationId);
  }

  isUserAllowedToChangeRole = computed(
    () => this.authService.userCurrentOrgRole() === 'Admin',
  );
  getOrganization(id: string) {
    this.organizationService.getOrganizationById(id).subscribe({
      next: (organization: Organization) => {
        this.organization = organization;
        this.isLoading = false;
        this.authService.userCurrentOrgRole.set(organization.currentUserRole);
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  addMember() {
    this.organizationService
      .addUserToOrganization(this.organizationId, this.memberEmail)
      .subscribe({
        next: () => {
          this.memberEmail = '';
          this.successMessage = 'Member added successfully!';
          this.getOrganization(this.organizationId);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to add member.';
        },
      });
  }

  private userService = inject(UserService);

  changeUserRole(userId: string, newRole: string) {
    this.userService
      .changeUserRole(userId, newRole, this.organizationId)
      .pipe(
        tap({
          next: () => {
            const current = this.authService.currentUser();
            if (current && current.id === userId) {
              this.authService.currentUser.set({ ...current, role: newRole });
              this.authService.refresh().subscribe();
            }
          },
        }),
        finalize(() => {
          this.getOrganization(this.organizationId);
        }),
      )
      .subscribe();
  }
}
