/// <reference types="jasmine" />
import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { roleGuard } from './role.guard';

describe('roleGuard', () => {
  const executeGuard = (allowedRoles: string[]) =>
    TestBed.runInInjectionContext(
      () => roleGuard(allowedRoles) as CanActivateFn,
    );

  const route = {} as ActivatedRouteSnapshot;
  const state = {} as RouterStateSnapshot;

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard(['Admin'])(route, state)).toBeTruthy();
  });
});
