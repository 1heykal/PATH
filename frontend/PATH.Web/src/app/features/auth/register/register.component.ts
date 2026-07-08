import { Component, inject, signal } from '@angular/core';
import { RegisterUserModel } from '../models/RegisterUserModel';
import { AuthService } from '../../../core/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
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

  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage = signal<string[] | null>(null);

  register() {
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

  isFormValid() {
    return (
      this.registerModel.FirstName.trim() !== '' &&
      this.registerModel.LastName.trim() !== '' &&
      this.registerModel.Email.trim() !== '' &&
      this.registerModel.DateOfBirth.trim() !== '' &&
      this.registerModel.Password.trim() !== '' &&
      this.registerModel.ConfirmPassword.trim() !== '' &&
      this.registerModel.Password === this.registerModel.ConfirmPassword
    );
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

  // checkFormValidity() {
  //   const errors: string[] = [];
  //   if (this.registerModel.FirstName.trim() === '') {
  //     errors.push('First name is required.');
  //   }
  //   if (this.registerModel.LastName.trim() === '') {
  //     errors.push('Last name is required.');
  //   }
  //   if (this.registerModel.Email.trim() === '') {
  //     errors.push('Email is required.');
  //   }
  //   if (this.registerModel.DateOfBirth.trim() === '') {
  //     errors.push('Date of birth is required.');
  //   }

  //   if (this.registerModel.Password.trim() === '') {
  //     errors.push('Password is required.');
  //   }

  //   if (this.registerModel.ConfirmPassword.trim() === '') {
  //     errors.push('Confirm password is required.');
  //   }

  //   if (this.registerModel.Password !== this.registerModel.ConfirmPassword) {
  //     errors.push('Passwords do not match.');
  //   }

  //   this.errorMessage.set(errors.length > 0 ? errors : null);
  //   return errors.length === 0;
  // }
}
