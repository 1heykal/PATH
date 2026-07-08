import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  constructor(public authService: AuthService) {}

  logout() {
    this.authService.logout().subscribe();
  }

  greeting = '';

  stats = {
    organizationCount: 0,
    projectCount: 0,
    taskCount: 0,
  };

  ngOnInit() {
    const hour = new Date().getHours();

    if (hour < 12) {
      this.greeting = 'Good morning';
    } else if (hour < 18) {
      this.greeting = 'Good afternoon';
    } else {
      this.greeting = 'Good evening';
    }

    this.stats = {
      organizationCount:
      this.authService.currentUser()?.organizationsCount ?? 0,
      projectCount: this.authService.currentUser()?.projectsCount ?? 0,
      taskCount: this.authService.currentUser()?.tasksCount ?? 0,
    };
  }

  // TODO:
  // this.dashboardService.getOverview().subscribe(...)
}
