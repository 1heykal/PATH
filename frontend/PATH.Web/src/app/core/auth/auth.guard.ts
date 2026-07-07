import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  let authService = inject(AuthService);
  if (authService.isLoggedIn()) return true;

  inject(Router).navigate(['login']);
  return false;
};
