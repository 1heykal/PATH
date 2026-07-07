import { Component, inject } from '@angular/core';
import { OrganizationService } from '../services/organization.service';
import { OrganizationBasicInfo } from '../models/OrganizationBasicInfo';
import { SpinnerComponent } from '../../../shared/spinner/spinner.component';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-organization-list',
  standalone: true,
  imports: [SpinnerComponent, RouterLink, DatePipe],
  templateUrl: './organization-list.component.html',
  styleUrl: './organization-list.component.scss',
})
export class OrganizationListComponent {
  private organizationService = inject(OrganizationService);
  organizations: OrganizationBasicInfo[] = [];

  isLoading: boolean = true;

  constructor() {
    this.getAllOrganizations();
  }

  getAllOrganizations() {
    this.organizationService
      .getAllOrganizations()
      .subscribe((organizations) => {
        this.organizations = organizations;
        this.isLoading = false;
      });
  }
}
