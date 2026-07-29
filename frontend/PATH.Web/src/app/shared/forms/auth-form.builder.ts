import { FormBuilder } from "@angular/forms";
import { AppValidators } from "../validators/app.validators";

export class AuthFormBuilder {
  static createLoginForm(fb: FormBuilder) {
    return fb.group({
      Email: ['', AppValidators.email],
      Password: ['', AppValidators.password],
    });
  }
}