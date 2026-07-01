import { Component, inject, OnInit, signal } from '@angular/core';
import { User } from '../models/User';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, tap } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { SpinnerComponent } from '../shared/spinner/spinner.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule, CommonModule, SpinnerComponent],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements OnInit {
  users = signal<User[]>([]);
  isLoading = signal<boolean>(true);
  private userService = inject(UserService);
  private authService = inject(AuthService);

  ngOnInit() {
    this.getUsers();
  }

  changeUserRole(userId: string, newRole: string) {
    this.userService
      .changeUserRole(userId, newRole)
      .pipe(
        tap({
          next: () => {
            const current = this.authService.currentUser();
            if (current && current.id === userId) {
              this.authService.currentUser.set({ ...current, role: newRole });
              this.authService.refresh().subscribe();
            }
          },
        }),
        finalize(() => {
          this.getUsers();
        }),
      )
      .subscribe();
  }

  getUsers() {
    this.userService.getUsers().subscribe({
      next: (data) => {
        this.users.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }
}
