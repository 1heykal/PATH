import { Component, inject, signal } from '@angular/core';
import { RegisterUserModel } from '../models/RegisterUserModel';
import { AuthService } from '../../../core/auth/auth.service';
import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { passwordMatch } from '../../../shared/validators/password-match.validator';
import { AppValidators } from '../../../shared/validators/app.validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  registerModel: RegisterUserModel = {
    FirstName: '',
    LastName: '',
    DateOfBirth: '',
    Email: '',
    Password: '',
    ConfirmPassword: '',
  };

  private fb = inject(FormBuilder);
  registerForm = this.fb.group(
    {
      firstName: ['', AppValidators.firstName],
      lastName: ['', AppValidators.lastName],
      dateOfBirth: ['', AppValidators.dateOfBirth],
      email: ['', AppValidators.email],
      password: ['', AppValidators.password],
      confirmPassword: ['', AppValidators.password],
    },
    { validators: [passwordMatch] },
  );

  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage = signal<string[] | null>(null);

  register() {
    this.registerModel = {
      FirstName: this.registerForm.controls.firstName.value ?? '',
      DateOfBirth: this.registerForm.controls.dateOfBirth.value ?? '',
      Email: this.registerForm.controls.email.value ?? '',
      LastName: this.registerForm.controls.lastName.value ?? '',
      Password: this.registerForm.controls.password.value ?? '',
      ConfirmPassword: this.registerForm.controls.confirmPassword.value ?? '',
    };

    const errors: string[] = [];
    if (this.registerModel.Password !== this.registerModel.ConfirmPassword) {
      errors.push('Passwords do not match.');
      this.errorMessage.set(errors);
      return;
    }
    this.authService.register(this.registerModel).subscribe({
      next: (response) => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage.set(
          this.normalizeErrors(
            err.error?.message,
            'Registration failed. Please try again.',
          ),
        );
      },
    });
  }

  checkPasswords() {
    if (this.registerModel.Password !== this.registerModel.ConfirmPassword) {
      const errors: string[] = ['Passwords do not match.'];
      this.errorMessage.set(errors);
    } else {
      this.errorMessage.set(null);
    }
    return this.registerModel.Password === this.registerModel.ConfirmPassword;
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
}
