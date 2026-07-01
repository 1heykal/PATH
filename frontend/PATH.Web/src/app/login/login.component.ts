import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AccessModel } from '../models/AccessModel';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  errorMessage = signal<string[] | null>(null);

  accessModel: AccessModel = {
    Email: '',
    Password: '',
  };

  login() {
    // if (!this.validateForm()) {
    //   return;
    // }
    this.authService.login(this.accessModel).subscribe({
      next: () => {
        this.errorMessage.set(null);
      },
      error: (err) => {
        this.errorMessage.set([
          err.error?.message || 'Login failed. Please try again.',
        ]);
        console.error('Login error:', err);
      },
    });
  }

  isFormValid() {
    return (
      this.accessModel.Email.trim() !== '' &&
      this.accessModel.Password.trim() !== ''
    );
  }

  // validateForm() {
  //   const errors: string[] = [];

  //   if (this.accessModel.Email.trim() === '') {
  //     errors.push('Email is required.');
  //   }
  //   if (this.accessModel.Password.trim() === '') {
  //     errors.push('Password is required.');
  //   }
  //   this.errorMessage.set(errors.length > 0 ? errors : null);
  //   return errors.length === 0;
  // }
}
