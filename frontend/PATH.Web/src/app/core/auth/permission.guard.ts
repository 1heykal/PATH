import { CanActivateFn, Router } from '@angular/router';
import { OrganizationService } from '../../features/organization/services/organization.service';
import { inject } from '@angular/core';
import { catchError, map, of, tap } from 'rxjs';
import { UserOrgPermissions } from '../../features/organization/models/UserOrgPermissions';

export const permissionGuard: CanActivateFn = (route, state) => {
  const permission = route.data['permission'] as keyof UserOrgPermissions;
  const orgId = route.queryParamMap.get('orgId');
  const router = inject(Router);

  if (!orgId) {
    router.navigate(['/dashboard']);
    return false;
  }

  const orgnizationService = inject(OrganizationService);

  return orgnizationService.getCurrentUserOrgMembership(orgId).pipe(
    map((membership) => {
      if (membership.permissions[permission]) {
        return true;
      }
      
      return router.createUrlTree(['/dashboard']);
    }),
    catchError(() => {
      return of(router.createUrlTree(['/dashboard']));
    }),
  );
};
