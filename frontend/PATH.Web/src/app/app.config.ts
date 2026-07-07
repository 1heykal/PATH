import {
  APP_INITIALIZER,
  ApplicationConfig,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { tokenInterceptor } from './core/auth/token.interceptor';
import { AuthService } from './core/auth/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([tokenInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AuthService],
      multi: true,
    },
  ],
};

export function initializeApp(authService: AuthService) {
  return () => authService.refresh();
}

// export function initializeApp(authService: AuthService, router: Router) {
//   return () => {
//     try {
//       authService.refresh();
//     } catch (err: any) {
//       if (err.status == 401) {
//         router.navigate(['/login']);
//       } else authService.refresh();
//     }
//     catchError(() => of(null));
//   };
// }
