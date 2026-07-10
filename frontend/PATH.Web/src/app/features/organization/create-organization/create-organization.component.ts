import { Component, inject } from '@angular/core';
import { OrganizationService } from '../services/organization.service';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-organization',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './create-organization.component.html',
  styleUrl: './create-organization.component.scss',
})
export class CreateOrganizationComponent {
  private orgService = inject(OrganizationService);
  private router = inject(Router);

  orgName = '';
  errorMessage: string | null = null;
  createOrganization() {
    this.orgService.createOrganization(this.orgName).subscribe({
      next: (org) => this.router.navigate(['/organizations', org.id]),
      error: (err) =>
        (this.errorMessage =
          err.error?.message || 'Failed to create organization.'),
    });
  }
}
