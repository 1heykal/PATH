import { Validators } from '@angular/forms';
import { notEmptyOrWhiteSpace } from './not-empty-or-whitespace.validtor';

export const AppValidators = {
  required: [Validators.required, notEmptyOrWhiteSpace],

  email: [Validators.email, Validators.required],

  password: [
    Validators.required,
    Validators.minLength(8),
    Validators.maxLength(100),
    notEmptyOrWhiteSpace,
  ],

  firstName: [
    Validators.required,
    Validators.minLength(2),
    Validators.maxLength(50),
    notEmptyOrWhiteSpace,
  ],

  lastName: [Validators.required, notEmptyOrWhiteSpace],

  dateOfBirth: [Validators.required],
};
