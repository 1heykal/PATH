import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  let authService = inject(AuthService);

  const addToken = (request: HttpRequest<unknown>) => {
    const accessToken = authService.getAccessToken();
    if (!accessToken) return request;

    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  };

  return next(addToken(req)).pipe(
    catchError((error) => {
      const execludedUrls = ['/Refresh', '/Login', '/Register'];
      const isExecluded = execludedUrls.some((url) => req.url.includes(url));

      if (error.status === 401 && !isExecluded)
        return authService.refresh().pipe(
          switchMap(() => next(addToken(req))),
          catchError(() => {
            authService.currentUser.set(null);
            inject(Router).navigate(['/login']);
            return throwError(() => error);
          }),
        );

      return throwError(() => error);
    }),
  );
};
