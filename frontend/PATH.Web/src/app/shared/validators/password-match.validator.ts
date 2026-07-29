import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';

export const passwordMatch: ValidatorFn = (
  formGroup: AbstractControl,
): ValidationErrors | null => {
  let passwordControl = formGroup.get('password');
  let confirmPasswordControl = formGroup.get('confirmPassword');

  if (
    !passwordControl ||
    !confirmPasswordControl ||
    !passwordControl.value ||
    !confirmPasswordControl.value
  ) {
    return null; // Don't validate if either control is missing or empty
  }

  return passwordControl.value === confirmPasswordControl.value
    ? null
    : { passwordMismatch: true };
};
