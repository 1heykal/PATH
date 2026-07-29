import { Validators } from '@angular/forms';

export const ProjectValidators = {
  name: [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(100),
  ],

  description: [Validators.maxLength(500)],
};
