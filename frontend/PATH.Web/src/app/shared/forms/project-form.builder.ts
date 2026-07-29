import { FormBuilder } from '@angular/forms';
import { ProjectValidators } from '../validators/project.validators';
import { Project } from '../../features/project/models/Project';

export class ProjectFormBuilder {
  static create(fb: FormBuilder) {
    return fb.group({
      name: ['', ProjectValidators.name],
      description: ['', ProjectValidators.description],
    });
  }

  static edit(fb: FormBuilder, project: Project) {
    return fb.group({
      name: [project.name, ProjectValidators.name],
      description: [project.description, ProjectValidators.description],
    });
  }
}
