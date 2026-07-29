import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
export const notEmptyOrWhiteSpace: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const value = control.value;

  if (typeof value === 'string' && value.trim().length === 0)
    return { emptyOrWhiteSpace: true };

  return null;
};
