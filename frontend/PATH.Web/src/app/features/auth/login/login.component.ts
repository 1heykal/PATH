import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';
import { Router } from '@angular/router';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AccessModel } from '../models/AccessModel';
import { CommonModule } from '@angular/common';
import { AppValidators } from '../../../shared/validators/app.validators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  errorMessage = signal<string[] | null>(null);

  accessModel: AccessModel = {
    Email: '',
    Password: '',
  };

  loginForm = this.fb.group({
    email: ['', AppValidators.email],
    password: ['', AppValidators.password],
  });

  login() {
    if (this.loginForm.valid) {
      this.accessModel = {
        Email: this.loginForm.controls.email.value ?? '',
        Password: this.loginForm.controls.password.value ?? '',
      };
    }
    this.authService.login(this.accessModel).subscribe({
      next: () => {
        this.errorMessage.set(null);
      },
      error: (err) => {
        this.errorMessage.set(
          this.normalizeErrors(
            err.error?.message,
            'Login failed. Please try again.',
          ),
        );
      },
    });
  }

  private normalizeErrors(
    error: string | string[] | undefined,
    fallback: string,
  ) {
    if (Array.isArray(error)) {
      return error.length > 0 ? error : [fallback];
    }

    if (typeof error === 'string' && error.trim() !== '') {
      return [error];
    }

    return [fallback];
  }

  isFormValid() {
    return (
      this.accessModel.Email.trim() !== '' &&
      this.accessModel.Password.trim() !== ''
    );
  }

  clicks = 0;
  private router = inject(Router);

  logoClicked() {
    this.clicks++;

    if (this.clicks >= 5) {
      this.router.navigate(['/secret-path']);
      this.clicks = 0;
    }

    setTimeout(() => {
      this.clicks = 0;
    }, 2000);
  }
}
