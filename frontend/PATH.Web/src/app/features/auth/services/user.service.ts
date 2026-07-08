import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private API_URL = `${environment.apiUrl}/users`;
  private http = inject(HttpClient);

  getUsers() {
    return this.http.get<User[]>(`${this.API_URL}`);
  }

  changeUserRole(userId: string, newRole: string, organizationId: string) {
    return this.http.patch(`${this.API_URL}/role`, {
      userId: userId,
      newRole: newRole,
      orgId: organizationId,
    });
  }
}
