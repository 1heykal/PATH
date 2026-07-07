import { Component, inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet, RouterLinkActive],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss',
})
export class LayoutComponent {
  authService = inject(AuthService);
  navOpen = false;

  logout() {
    this.authService.logout().subscribe();
  }
}
