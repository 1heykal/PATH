import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateOrganizationResponse } from '../models/CreateOrganizationResponse';
import { environment } from '../../../../environments/environment';
import { Organization } from '../models/Organization';
import { OrganizationBasicInfo } from '../models/OrganizationBasicInfo';
import { OrgMember } from '../models/OrgMember';
import { UserOrgMembership } from '../models/UserOrgMembership';
@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  private API_URL = `${environment.apiUrl}/organizations`;
  private http = inject(HttpClient);

  getAllOrganizations() {
    return this.http.get<OrganizationBasicInfo[]>(this.API_URL);
  }

  getOrganizationById(id: string) {
    return this.http.get<Organization>(`${this.API_URL}/${id}`);
  }

  createOrganization(name: string) {
    return this.http.post<CreateOrganizationResponse>(this.API_URL, {
      name: name,
    });
  }

  getOrgMembers(id: string) {
    return this.http.get<OrgMember[]>(`${this.API_URL}/${id}/members`);
  }

  addUserToOrganization(organizationId: string, userEmail: string) {
    return this.http.post<void>(`${this.API_URL}/members`, {
      userEmail: userEmail,
      organizationId: organizationId,
    });
  }

  getCurrentUserOrgMembership(id: string) {
    return this.http.get<UserOrgMembership>(`${this.API_URL}/${id}/me`);
  }
}
